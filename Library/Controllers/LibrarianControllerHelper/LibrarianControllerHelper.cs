using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.Cloudinary;
using Library.Models.DTO;
using Library.Models.UserModels.Interfaces;
using Library.Models.ViewModels;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class LibrarianControllerHelper : ILibrarianControllerHelper
    {
        #region FieldsAndConstructors
        private readonly IBookService _bookService;
        private readonly IBookCategoryService _bookCategoryService;
        private readonly IBookSubjectService _bookSubjectService;
        private readonly IMembershipService _membershipService;
        private readonly IUserLeasedBookService _userLeasedBookService;
        private readonly IEmailSenderService _emailSenderService;

        private UserManager<ApplicationUser> _userManager;

        public LibrarianControllerHelper(IBookService bookService, IBookCategoryService bookCategoryService,
            IUserLeasedBookService userLeasedBookService, IBookSubjectService bookSubjectService, IEmailSenderService emailSenderService,
            IMembershipService membershipService, UserManager<ApplicationUser> userManager)
        {
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
            _bookSubjectService = bookSubjectService;
            _membershipService = membershipService;
            _userLeasedBookService = userLeasedBookService;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
        }
        #endregion

        #region BooksHelpers
        public async Task<BookChangersViewModel> AddABookHelper()
        {
            var viewModel = new BookChangersViewModel();
            viewModel.AllGenres = _bookCategoryService.IQueryableGetAllAsync().Select(x => x.CategoryName);
            viewModel.AllMemberships = _membershipService.IQueryableGetAllAsync().Select(x => x.MembershipName);

            return viewModel;
        }

        public async Task<bool> AddABookToDatabase(BookChangersViewModel book, string image)
        {
            try
            {
                var bookCat = _bookCategoryService.GetBookCategoryByBookCategoryName(book!.Genre!);
                var bookNew = new Book();

                bookNew.Id = book.Id;
                bookNew.Title = book.Name;
                bookNew.Author = book.Author;
                bookNew.DateOfBookCreation = book.DateOfBookCreation;
                bookNew.Genre = bookCat;
                bookNew.AmountOfBooks = book.AmountOfBooks;
                bookNew.Description = book.Description;
                bookNew.NeededMembership = _membershipService.GetMembershipByName(book.NeededMembership);

                var id = await _bookService.AddAsync(bookNew);

                var photo = new Photo();
                if (image != null && image != "")
                {
                    photo.Image = image;
                    photo.ImageName = $"image-for-book-{id}";
                    photo.PublicId = $"image-for-book-{id}";
                }
                await _bookService.DeleteImage(photo);
                await _bookService.SaveImage(photo);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<string> AddBookSubjectAndCategoriesToDb(List<BookSubjectDTO> bookSubjectsDTO, List<BookCategoryDTO> bookCategoriesDTO)
        {
            try
            {
                string out2 = await RemoveAllCategoriesThatAreNotInTheDTO(bookCategoriesDTO);
                string out1 = await RemoveAllSubjectsThatAreNotInTheDTO(bookSubjectsDTO);

                if (!String.IsNullOrEmpty(out1) || !String.IsNullOrEmpty(out2))
                {
                    return out1 + out2;
                }

                foreach (var subject in bookSubjectsDTO)
                {
                    var subjectObj = await _bookSubjectService
                        .IQueryableGetAllAsync()
                        .Where(x => x.SubjectName == subject.SubjectName).Include(x => x.BookCategories)
                        .FirstOrDefaultAsync();

                    if (subjectObj != null)
                    {
                        var nonExistentBookCategories = new List<BookCategory>();
                        foreach (var category in bookCategoriesDTO.Where(x => x.SubjectName == subjectObj.SubjectName))
                        {
                            var categoryObj = await _bookCategoryService.IQueryableGetAllAsync().Where(x => x.CategoryName == category.CategoryName).FirstOrDefaultAsync();

                            if (categoryObj == null)
                            {
                                var catObj = new BookCategory();
                                catObj.CategoryName = category!.CategoryName;

                                var entity = await _bookCategoryService.AddAsync(catObj);
                                catObj.Id = entity;

                                nonExistentBookCategories.Add(catObj);
                            }
                        }

                        foreach (var cat in nonExistentBookCategories)
                        {
                            subjectObj.BookCategories.Add(cat);
                        }
                        await _bookSubjectService.UpdateAsync(subjectObj);
                    }

                    else
                    {
                        var bSubj = new BookSubject();
                        bSubj.SubjectName = subject.SubjectName;

                        var bookCategories = new List<BookCategory>();
                        foreach (var category in bookCategoriesDTO.Where(x => x.SubjectName == subject.SubjectName))
                        {
                            var bCat = new BookCategory();
                            bCat.CategoryName = category.CategoryName;

                            var entityId = await _bookCategoryService.AddAsync(bCat);

                            bookCategories.Add(await _bookCategoryService.GetByIdAsync(entityId));
                        }
                        bSubj.BookCategories = bookCategories;
                        await _bookSubjectService.AddAsync(bSubj);
                    }
                }
                return "";

            }
            catch (Exception)
            {
                return "An error occured!";
                throw;
            }
        }

        private async Task<string> RemoveAllSubjectsThatAreNotInTheDTO(List<BookSubjectDTO> bookSubjectDTOs)
        {
            var dbSubjects = await _bookSubjectService.GetAllAsync();
            var output = "";
            var subjectsToRemove = dbSubjects
                .Where(dbSubject => !bookSubjectDTOs
                    .Any(dto => string.Equals(dto.SubjectName, dbSubject.SubjectName, StringComparison.OrdinalIgnoreCase)))
                 .ToList();

            foreach (var subjectToRemove in subjectsToRemove)
            {

                foreach (var categ in subjectToRemove.BookCategories)
                {
                    if (!_bookService.IQueryableGetAllAsync().Where(x => x.Genre == categ).Any())
                    {
                        await _bookCategoryService.RemoveAsync(categ.Id);
                    }
                    else
                    {
                        output += $"The category: {categ.CategoryName} has books connected to it and cannot be deleted!-";
                    }
                }
                if (subjectToRemove.BookCategories.Count() == 0)
                {
                    await _bookSubjectService.RemoveAsync(subjectToRemove.Id);
                }
                else
                {
                    output += $"The subject: {subjectToRemove.SubjectName} has books connected to it and cannot be deleted!-";
                }

            }
            return output;
        }

        private async Task<string> RemoveAllCategoriesThatAreNotInTheDTO(List<BookCategoryDTO> bookCategoryDTOs)
        {
            var dbCategories = await _bookCategoryService.GetAllAsync();
            var output = "";
            var categoriesToRemove = dbCategories
                .Where(dbCategory =>
                    !bookCategoryDTOs.Any(dto =>
                        string.Equals(dto.CategoryName, dbCategory.CategoryName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // Remove identified categories
            foreach (var categoryToRemove in categoriesToRemove)
            {
                if (!_bookService.IQueryableGetAllAsync().Where(x => x.Genre == categoryToRemove).Any())
                {
                    await _bookCategoryService.RemoveAsync(categoryToRemove.Id);
                }
                else
                {
                    output += $"The category: {categoryToRemove.CategoryName} has books connected to it and cannot be deleted!-";
                }
            }
            return output;
        }

        public async Task<BookChangersViewModel> EditABookHelper(Guid id)
        {
            var book = await GetBook(id);
            var viewModel = new BookChangersViewModel();
            viewModel.Id = book.Id;
            viewModel.Name = book.Title;
            viewModel.Author = book.Author;
            viewModel.DateOfBookPublishment = book.DateOfBookPublishment;
            viewModel.DateOfBookCreation = book.DateOfBookCreation;
            viewModel.Genre = book.Genre.CategoryName;
            viewModel.Description = book.Description;
            viewModel.AmountOfBooks = book.AmountOfBooks;
            viewModel.NeededMembership = book.NeededMembership.MembershipName;
            viewModel.AllGenres = _bookCategoryService.IQueryableGetAllAsync().Select(x => x.CategoryName);
            viewModel.AllMemberships = _membershipService.IQueryableGetAllAsync().Select(x => x.MembershipName);

            return viewModel;
        }

        public async Task<bool> EditABook(BookChangersViewModel book, string imageObj)
        {
            try
            {
                var bookCat = _bookCategoryService.GetBookCategoryByBookCategoryName(book.Genre!);

                var bookNew = new Book();

                bookNew.Id = book.Id;
                bookNew.Title = book.Name;
                bookNew.Author = book.Author;
                bookNew.DateOfBookCreation = book.DateOfBookCreation;
                bookNew.Genre = bookCat;
                bookNew.Description = book.Description;
                bookNew.AmountOfBooks = book.AmountOfBooks;
                bookNew.NeededMembership = _membershipService.GetMembershipByName(book.NeededMembership);


                await _bookService.UpdateAsync(bookNew);

                var photo = new Photo();
                if (imageObj != null && imageObj != "")
                {
                    photo.Image = imageObj;
                    photo.ImageName = $"image-for-book-{book.Id}";
                    photo.PublicId = $"image-for-book-{book.Id}";
                }
                await _bookService.DeleteImage(photo);
                await _bookService.SaveImage(photo);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public IQueryable<BookSubject> GetAllBookSubjects()
        {
            return _bookSubjectService.IQueryableGetAllAsync();
        }

        public async Task<Book> GetBook(Guid bookId)
        {
            return await _bookService.GetByIdAsync(bookId);
        }

        public IQueryable<Book> GetAllBooks()
        {
            return _bookService.IQueryableGetAllAsync();
        }

        public async Task<int> RemoveABook(Guid bookId)
        {
            return await _bookService.RemoveAsync(bookId);
        }

        public async Task SaveBookHelper(BookViewModelDTO viewModelDTO)
        {
            var bookEntity = new Book();
            bookEntity.ISBN = viewModelDTO.ISBN;
            bookEntity.Title = viewModelDTO.Title;
            bookEntity.Author = viewModelDTO.Authors;
            bookEntity.DateOfBookPublishment = DateTime.Now;
            bookEntity.DateOfBookCreation = viewModelDTO.PublishDate;
            bookEntity.Language = viewModelDTO.Language;
            bookEntity.AmountOfBooks = viewModelDTO.AmountOfBooks;
            bookEntity.NeededMembership = _membershipService.IQueryableGetAllAsync().OrderBy(x => x.StartingNeededAmountOfPoints).FirstOrDefault();
            bookEntity.Genre = _bookCategoryService.GetBookCategoryByBookCategoryName(viewModelDTO.Category);

            var neededId = await _bookService.AddAsync(bookEntity);

            var photo = new Photo();
            if (viewModelDTO.ImageURL != null && viewModelDTO.ImageURL != "")
            {
                photo.Image = viewModelDTO.ImageURL; 
                photo.ImageName = $"image-for-book-{neededId}";
                photo.PublicId = $"image-for-book-{neededId}";
            }

            await _bookService.SaveImage(photo);

        }

        public async Task<List<string>> GetAllGenresHelper()
        {
            return await _bookCategoryService.IQueryableGetAllAsync().Select(x => x.CategoryName).ToListAsync();
        }
        public async Task<LeasedTrackerViewModel> GetLeasedTrackerData(string Category)
        {
            var viewModel = new LeasedTrackerViewModel();
            if (Category == "Active")
            {
                viewModel.LeasedBooks = _userLeasedBookService.GetActiveLeasedBooks();
            }
            else if (Category == "Expired")
            {
                viewModel.LeasedBooks = _userLeasedBookService.GetExpiredLeasedBooks();
            }
            else
            {
                viewModel.LeasedBooks = _userLeasedBookService.IQueryableGetAllAsync().Where(x => x.Approved == false);
            }
            viewModel.Category = Category;
            return viewModel;
        }

        #endregion

        #region ReportsHelper
        public async Task<ReportViewModel> GetReportPageModel()
        {
            var viewModel = new ReportViewModel();

            viewModel.AmountOfUsers = _userManager.Users.Count();
            viewModel.AmountOfBooks = await _bookService.IQueryableGetAllAsync().CountAsync();
            viewModel.AmountOfCategories = await _bookCategoryService.IQueryableGetAllAsync().CountAsync();
            viewModel.AmountOfSubjects = await _bookSubjectService.IQueryableGetAllAsync().CountAsync();
            viewModel.AmountOfLeased = await _userLeasedBookService.IQueryableGetAllAsync().Where(x=>!x.IsRead&&x.Approved).CountAsync();

            var leasedBook = await _userLeasedBookService.GetBooksInformationByTimeAndCountOfItems(DateTime.Now.AddHours(-24), DateTime.Now, 1);

            foreach (var item in leasedBook)
            {
                var mostLeasedBook = new ReportBookDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                viewModel.MostLeasedBook = mostLeasedBook;
            }
            viewModel.MostReadGenres = await _userLeasedBookService.MostReadGenres(DateTime.Now.AddHours(-24), DateTime.Now);
            return viewModel;
        }

        public async Task<IEnumerable<ReportBookDTO>> GetBookInformationByTimeAndCount(DateTime startDate, DateTime endDate, int selectedCountOfItems)
        {
            return await _userLeasedBookService.GetBooksInformationByTimeAndCountOfItems(startDate, endDate, selectedCountOfItems);
        }

        public async Task<List<string>> GetGenreInformationByTimeAndCount(DateTime startDate, DateTime endDate)
        {
            return await _userLeasedBookService.MostReadGenres(startDate, endDate);
        }

        #endregion

        #region LeasingBooksHelper
        public async Task<bool> LeaseBookOrNotHelper(Guid userLeasedId, bool lease)
        {
            try
            {
                if (lease)
                {
                    var entity = _userLeasedBookService.IQueryableGetAllAsync().Where(x => x.Id == userLeasedId).FirstOrDefault();
                    if (entity != null)
                    {
                        if (entity.Book.AmountOfBooks > 0)
                        {
                            var user = entity.User;
                            var emailBody = $@"
                         <html>
                           <body>
                               <div style='text-align: center;'>
                                   <img src='https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/litify-logo_fwtfvq' alt='Website Logo' style='height: 20em;' />
                               </div>
                               <div style='text-align: center; font-size: 22px; font-weight: bold; margin-top: 20px; text-decoration:none; color: black;'>
                                   {user.UserName},<br/> Вашата поръчка беше приета!
                               </div>
                               <br/>
                               <hr/>
                               <br/>
                               <div style='background:#ffffff;background-color:#ffffff;margin:0px auto;font-family:Arial,sans-serif;max-width:864px'>
                                   <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#ffffff;background-color:#ffffff;width:100%;font-family:Arial,sans-serif' width='100%' bgcolor='#ffffff'>
                                       <tbody>
                                           <tr style='font-family:Arial,sans-serif'>
                                               <td style='direction:ltr;font-size:0px;padding:32px 8px 16px 8px;text-align:center;font-family:Arial,sans-serif' align='center'>
                                                   <div class='m_-6124457663806841501mj-column-per-100' style='font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;font-family:Arial,sans-serif'>
                                                       <table border='0' cellpadding='0' cellspacing='0' role='presentation' width='100%' style='font-family:Arial,sans-serif'>
                                                           <tbody>
                                                               <tr style='font-family:Arial,sans-serif'>
                                                                   <td style='vertical-align:top;padding:0;font-family:Arial,sans-serif' valign='top'>
                                                                       <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='font-family:Arial,sans-serif' width='100%'>
                                                                           <tbody>
                                                                               <tr style='font-family:Arial,sans-serif'>
                                                                                   <td align='none' style='font-size:0px;padding:0px 0px 24px 0px;word-break:break-word;font-family:Arial,sans-serif'>
                                                                                       <div style='font-size:16px;font-weight:900;line-height:18px;text-align:none;color:#000000;font-family:Arial,sans-serif'>
                                                                                           Книги:
                                                                                       </div>
                                                                                   </td>
                                                                               </tr>
                                                                                <hr/>
                                                                                <tr style='width:100%;white-space:nowrap;font-family:Arial,sans-serif'>
                                                                                    <td style='border-top:1px solid #ededed;font-size:12px;line-height:20px;width:200px;max-width:250px;font-family:Arial,sans-serif' width='200'></td>
                                                                                </tr>
                                                                                <tr style='font-family:Arial,sans-serif'>
                                                                                    <td align='none' style='font-size:0px;padding:12px 0;word-break:break-word;font-family:Arial,sans-serif'>
                                                                                        <div style='font-size:13px;line-height:1;text-align:none;color:#000000;font-family:Arial,sans-serif'>
                                                                                            <table style='width:100%;font-family:Arial,sans-serif' width='100%'>
                                                                                                <tbody>
                                                                                                    <tr style='font-family:Arial,sans-serif'>
                                                                                                        <td rowspan='6' style='height:13em;min-width:8.6em;padding-right:24px;font-family:Arial,sans-serif' height='13em'>
                                                                                                            <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='border-collapse:collapse;border-spacing:0px;font-family:Arial,sans-serif'>
                                                                                                                <tbody>
                                                                                                                    <tr style='font-family:Arial,sans-serif'>
                                                                                                                        <td style='width:8.6em;font-family:Arial,sans-serif' width='72'>
                                                                                                                            <a href='' style='text-decoration:underline;height:13em;min-width:72px;font-family:Arial,sans-serif;color:black' target='_blank' data-saferedirecturl=''>
                                                                                                                                <img src='https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/image-for-book-{entity.Book.Id}.png' style='border:0;display:block;outline:none;text-decoration:none;height:13em;width:100%' width='8.6em' class='CToWUd' data-bit='iit'>
                                                                                                                            </a>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                </tbody>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                        <td style='font-size:18px;font-weight:bold;line-height:20px;width:37em;max-width:37em;font-family:Arial,sans-serif' width='37em'>
                                                                                                              {entity.Book.Title}
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr style='white-space:nowrap;font-family:Arial,sans-serif'>
                                                                                                        <td style='height:8px;font-family:Arial,sans-serif' height='8'></td>
                                                                                                    </tr>
                                                                                                    <tr style='white-space:nowrap;font-family:Arial,sans-serif'>
                                                                                                        <td style='font-size:15px;line-height:20px;width:37em;max-width:37em;font-family:Arial,sans-serif' width='37em'>
                                                                                                            {entity.Book.Author}
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr style='white-space:nowrap;font-family:Arial,sans-serif'>
                                                                                                        <td style='font-size:15px;line-height:20px;width:37em;max-width:37em;font-family:Arial,sans-serif' width='37em'>
                                                                                                            {entity.Book.DateOfBookCreation.Year} г.
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </tbody>
                                                                                            </table>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr style='width:100%;white-space:nowrap;font-family:Arial,sans-serif'>
                                                                                    <td style='border-bottom:1px solid #ededed;font-size:12px;line-height:20px;width:200px;max-width:250px;font-family:Arial,sans-serif' width='200'></td>
                                                                                </tr>
                                                                           </tbody>
                                                                       </table>
                                                                   </td>
                                                               </tr>
                                                           </tbody>
                                                       </table>
                                                   </div>
                                                   <div style='background:#ffffff;background-color:#ffffff;margin:0px auto;font-family:Arial,sans-serif;max-width:864px'>
                                                       <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#ffffff;background-color:#ffffff;width:100%;font-family:Arial,sans-serif' width='100%' bgcolor='#ffffff'>
                                                           <tbody>
                                                               <tr style='font-family:Arial,sans-serif'>
                                                                   <td style='direction:ltr;font-size:0px;padding:20px 8px 22px 8px;text-align:right;font-family:Arial,sans-serif' align='right'>
                                                                       <div class='m_-6124457663806841501mj-column-per-100' style='font-size:0px;text-align:right;direction:ltr;display:inline-block;vertical-align:top;width:100%;font-family:Arial,sans-serif'>
                                                                           <table border='0' cellpadding='0' cellspacing='0' role='presentation' width='100%' style='font-family:Arial,sans-serif'>
                                                                               <tbody>
                                                                                   <tr style='font-family:Arial,sans-serif'>
                                                                                       <td style='vertical-align:top;padding:0;font-family:Arial,sans-serif' valign='top'>
                                                                                           <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='font-family:Arial,sans-serif' width='100%'>
                                                                                               <tbody>
                                                                                                   <tr style='font-family:Arial,sans-serif'>
                                                                                                       <td align='none' style='font-size:0px;padding:0;word-break:break-word;font-family:Arial,sans-serif'>
                                                                                                           <table cellpadding='0' cellspacing='0' width='100%' border='0' style='color:#000000;font-size:13px;line-height:22px;table-layout:auto;width:100%;border:none;font-family:Arial,sans-serif'>
                                                                                                               <tbody>
                                                                                                                   <tr style='font-family:Arial,sans-serif'>
                                                                                                                       <td style='text-align: left;font-size:15px;line-height:20px;width: 100%;font-family:Arial,sans-serif' width='100%'>
                                                                                                                           *Вашата книга е добавена като готова за четене, отидете в страницата 'Взети назаем (първия бутон до логото, в приложението)' и натиснете 'Чети книга'.
                                                                                                                       </td>
                                                                                                                    
                                                                                                                   </tr>
                                                                                                                    <tr style='font-family:Arial,sans-serif'>
                                                                                                                       <td style='text-align: left;font-size:15px;line-height:20px;width: 100%;font-family:Arial,sans-serif' width='100%'>
                                                                                                                           При натискането на бутона ще имате определено време за четене (зависещо от Вашия ранг в приложението).
                                                                                                                       </td>
                                                                                                                    </tr>
                                                                                                               </tbody>
                                                                                                           </table>
                                                                                                       </td>
                                                                                                   </tr>
                                                                                               </tbody>
                                                                                           </table>
                                                                                       </td>
                                                                                   </tr>
                                                                               </tbody>
                                                                           </table>
                                                                       </div>
                                                                   </td>
                                                               </tr>
                                                           </tbody>
                                                       </table>
                                                   </div>
                                               </td>
                                           </tr>
                                       </tbody>
                                   </table>
                               </div>
                            </body>
                        </html>";

                            var subjectText = "Потвърждение на отдаване!";

                            _emailSenderService.SendEmail(entity.User!.Email!, emailBody, subjectText);

                            entity.Approved = true;
                            entity.Book.AmountOfBooks--;
                            await _userLeasedBookService.UpdateAsync(entity);
                        }
                    }
                }
                else
                {
                    await _userLeasedBookService.RemoveAnHourToExistingEntity(userLeasedId);
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }

        public async Task<bool> StopLeasingHelper(Guid userLeasedId)
        {
            try
            {
                await _userLeasedBookService.RemoveAnHourToExistingEntity(userLeasedId);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }

        public async Task<bool> DeleteLeasedEntityHelper(Guid id)
        {
            try
            {
                await _userLeasedBookService.RemoveAsync(id);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }
        #endregion
    }
}
