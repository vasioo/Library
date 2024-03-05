using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.Cloudinary;
using Library.Models.DTO;
using Library.Models.ViewModels;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        private UserManager<ApplicationUser> _userManager;

        public LibrarianControllerHelper(IBookService bookService, IBookCategoryService bookCategoryService,
            IUserLeasedBookService userLeasedBookService, IBookSubjectService bookSubjectService,
            IMembershipService membershipService, UserManager<ApplicationUser> userManager)
        {
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
            _bookSubjectService = bookSubjectService;
            _membershipService = membershipService;
            _userLeasedBookService = userLeasedBookService;
            _userManager = userManager;
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
            bookEntity.Subtitle = viewModelDTO.Subtitle;
            bookEntity.Author = viewModelDTO.Authors;
            bookEntity.DateOfBookPublishment = DateTime.Now;
            bookEntity.DateOfBookCreation = viewModelDTO.PublishDate;
            bookEntity.Language = viewModelDTO.Language;
            bookEntity.Genre = _bookCategoryService.GetBookCategoryByBookCategoryName(viewModelDTO.Category);

            await _bookService.AddAsync(bookEntity);
        }

        public async Task<List<string>> GetAllGenresHelper()
        {
            return await _bookCategoryService.IQueryableGetAllAsync().Select(x=>x.CategoryName).ToListAsync();
        }
        #endregion

        #region ReportsHelper
        public async Task<ReportViewModel> GetReportPageModel()
        {
            var viewModel = new ReportViewModel();

            viewModel.AmountOfUsers = _userManager.Users.Count();

            var leasedBook = await _userLeasedBookService.GetBooksInformationByTimeAndCountOfItems(DateTime.Now.AddHours(-24),DateTime.Now,1);

            foreach (var item in leasedBook)
            {
                var mostLeasedBook = new ReportBookDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                viewModel.MostLeasedBook = mostLeasedBook;
            }
            viewModel.MostReadGenres = await _userLeasedBookService.MostReadGenres(DateTime.Now.AddHours(-24),DateTime.Now);
            return viewModel;
        }

        public async Task<IEnumerable<ReportBookDTO>> GetBookInformationByTimeAndCount(DateTime startDate,DateTime endDate, int selectedCountOfItems)
        {
            return await _userLeasedBookService.GetBooksInformationByTimeAndCountOfItems(startDate,endDate, selectedCountOfItems);
        }

        public async Task<List<string>> GetGenreInformationByTimeAndCount(DateTime startDate, DateTime endDate)
        {
            return await _userLeasedBookService.MostReadGenres(startDate, endDate);
        }

        
        #endregion
    }
}
