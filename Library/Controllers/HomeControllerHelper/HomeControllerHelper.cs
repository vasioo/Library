using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.ViewModels;
using Library.Services.Interfaces;
using Library.Services.Services;
using Microsoft.AspNetCore.Identity;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class HomeControllerHelper : IHomeControllerHelper
    {
        #region Fields&Constructor
        private readonly IBookService _bookService;
        private readonly IDocumentService _documentsService;
        private readonly IBookCategoryService _bookCategoryService;
        private readonly IUserLeasedBookService _userLeasedBookService;
        private readonly IBookSubjectService _bookSubjectService;
        private readonly IStarRatingService _starRatingService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IMembershipService _membershipService;


        public HomeControllerHelper(IBookSubjectService bookSubjectService,
            IBookService bookService, IEmailSenderService emailSenderService,
            IStarRatingService starRatingService, IBookCategoryService bookCategoryService,
            IUserLeasedBookService userLeasedBookService, IDocumentService documentsService,
            IMembershipService membershipService)
        {
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
            _userLeasedBookService = userLeasedBookService;
            _bookSubjectService = bookSubjectService;
            _documentsService = documentsService;
            _starRatingService = starRatingService;
            _emailSenderService = emailSenderService;
            _membershipService = membershipService;

        }
        #endregion

        #region BookCollectionShowerHelper
        public BookCollectionShowerViewModel GetBookCollectionAttributes(ApplicationUser user)
        {
            var viewModel = new BookCollectionShowerViewModel();

            viewModel.BookSubjects = _bookSubjectService.IQueryableGetAllAsync().OrderBy(x => x.SubjectName);
            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }
        #endregion

        #region BookShowerHelper
        public BookShowerViewModel GetBooksAttributes(ApplicationUser user, string category)
        {
            var viewModel = new BookShowerViewModel();

            var cat = _bookCategoryService.IQueryableGetAllAsync().Where(x => x.CategoryName == category).FirstOrDefault();

            viewModel.CategorySortBy = cat == null ? new BookCategory() : cat;
            viewModel.Books = _bookService.IQueryableGetAllAsync().Where(x => x.Genre == viewModel.CategorySortBy);

            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");


            return viewModel;
        }
        #endregion

        #region MainPageHelper
        public MainPageViewModel GetMainPageAttributes(ApplicationUser user)
        {
            //RecurringJob.AddOrUpdate(() => _notificationService.AddDailyNotification(), "0 14 * * *", TimeZoneInfo.Local);
            //RecurringJob.AddOrUpdate(() => _notificationService.AddWeeklyNotification(), "0 17 * * 0", TimeZoneInfo.Local);
            var viewModel = new MainPageViewModel();
            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }
        #endregion

        #region BookPageHelper

        public async Task<BookPageViewModel> GetBookPageAttributes(ApplicationUser user, Guid bookId)
        {
            var viewModel = new BookPageViewModel();

            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            viewModel.User = user;
            viewModel.Book = await _bookService.GetByIdAsync(bookId);
            viewModel.UserIsAuthorized = user.Points > _membershipService.GetMembershipByName(viewModel.Book.NeededMembership.MembershipName)
                .StartingNeededAmountOfPoints;
            var starRating = _starRatingService.IQueryableGetAllAsync()
                .Where(x => x.User == user && x.Book == viewModel.Book)
                .FirstOrDefault();
            if (starRating!=null)
            {
                viewModel.StarRatingAmount = starRating.StarCount;
            }
            else
            {
                viewModel.StarRatingAmount = 0;
            }
            if (user != null)
            {
                var borrowedBook = await _userLeasedBookService.GetBorrowedBookByUserIdAndBookId(bookId, user.Id);
                if (borrowedBook!.Id != Guid.Empty)
                {
                    viewModel.HasUserBorrowedIt = true;
                    if (!borrowedBook.Approved && !borrowedBook.IsRead)
                    {
                        viewModel.IsWaiting = true;
                    }
                }
                else
                {
                    viewModel.HasUserBorrowedIt = false;
                }
                var status = await _userLeasedBookService.GetLeasedBookStatus(bookId, user);
                if (status == "Expired")
                {
                    viewModel.IsDisabled = true;
                }
                else
                {
                    viewModel.IsDisabled = false;
                }
            }
            return viewModel;
        }

        public async Task<bool> BorrowBookPostHelper(Guid bookId, ApplicationUser user)
        {
            try
            {
                var book = await _bookService.GetByIdAsync(bookId);
                if (book != null && !string.IsNullOrEmpty(user.Id))
                {
                    var userLeasedBook = new UserLeasedBookMappingTable();

                    userLeasedBook.Book = book;
                    userLeasedBook.User = user;
                    userLeasedBook.DateOfBorrowing = DateTime.Now;
                    if (_userLeasedBookService.GetBorrowedBookByUserIdAndBookId(bookId, user.Id) != null)
                    {
                        await _userLeasedBookService.AddAsync(userLeasedBook);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> UnborrowBookPostHelper(Guid bookId, string userId)
        {
            try
            {
                if (bookId != Guid.Empty && !string.IsNullOrEmpty(userId))
                {
                    var borrowedBook = await _userLeasedBookService.GetBorrowedBookByUserIdAndBookId(bookId, userId);
                    if (borrowedBook!.Id != Guid.Empty)
                    {
                        await _userLeasedBookService.RemoveAsync(borrowedBook.Id);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<string> UpdateUsersReadAttribute(ApplicationUser user, string isbn)
        {
            try
            {
                var book = _bookService.IQueryableGetAllAsync().Where(x => x.ISBN == isbn).FirstOrDefault();
                var mappingElement = _userLeasedBookService.IQueryableGetAllAsync().Where(x=>x.Book.Id==book!.Id&&x.User==user).FirstOrDefault();
                if (!mappingElement!.IsRead)
                {
                    mappingElement.IsRead = true;
                }
                if (!String.IsNullOrEmpty(book!.BookPreviewLink))
                {
                    return book.BookPreviewLink;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
                throw;
            }
        }

        public async Task UpdateBookLink(string isbn,string link)
        {
            var book = _bookService.IQueryableGetAllAsync().Where(x => x.ISBN == isbn).FirstOrDefault();
            book!.BookPreviewLink = link;
            await _bookService.UpdateAsync(book);
        }
        #endregion

        #region BorrowedHelper

        public BorrowedViewModel GetBorrowedPageAttributes(ApplicationUser user)
        {
            var viewModel = new BorrowedViewModel();
            if (user != null)
            {
                var allLeasedBooks = _userLeasedBookService.IQueryableGetAllAsync();
                viewModel.BorrowedBooks = allLeasedBooks.Where(us => us.User.Id == user.Id);
            }
            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user!, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }

        #endregion

        #region SearchPageHelper

        public async Task<SearchViewModel> SearchViewModelHelper(string searchCategory, string inputValue, int page = 1)
        {
            var viewModel = new SearchViewModel();

            if (searchCategory == null)
            {
                searchCategory = "";
            }

            if (inputValue == null)
            {
                inputValue = "";
            }

            if (searchCategory == "Documents")
            {
                var documents = _documentsService.IQueryableGetAllAsync()
                    .Where(x => x.Title.Contains(inputValue) || x.Content.Contains(inputValue))
                    .Skip((page - 1) * 20).Take(20);
                viewModel.TotalPages = (int)Math.Ceiling((double)_documentsService.IQueryableGetAllAsync().Count() / 20);
                viewModel.PageNumber = page;
                var authorDTO = documents.Select(blogPost => new Document
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    DateOfCreation = blogPost.DateOfCreation,
                    Content = blogPost.Content,
                }).AsQueryable();

                viewModel.BlogPosts = authorDTO;
            }

            else if (searchCategory == "Subjects")
            {
                var subjects = _bookSubjectService.IQueryableGetAllAsync()
                    .Where(x => x.SubjectName.Contains(inputValue))
                    .Skip((page - 1) * 20).Take(20);
                viewModel.TotalPages = (int)Math.Ceiling((double)_bookSubjectService.IQueryableGetAllAsync().Count() / 20);
                viewModel.PageNumber = page;
                var subjectDTOs = subjects.Select(subject => new SubjectDTO
                {
                    Subject = subject.SubjectName,
                    AmountOfBooksWithinThatSubject = _bookSubjectService.GetBookCountByBookSubject(subject)
                }).AsQueryable();


                viewModel.Subjects = subjectDTOs;
            }

            else
            {
                var books = _bookService.IQueryableGetAllAsync()
                    .Where(x => x.Title.Contains(inputValue)||x.Author.Contains(inputValue)||x.ISBN.Contains(inputValue))
                    .Skip((page - 1) * 20).Take(20);
                viewModel.TotalPages = (int)Math.Ceiling((double)_bookService.IQueryableGetAllAsync().Count() / 20);
                viewModel.PageNumber = page;
                var bookDTOs = books.Select(book => new BookDTO
                {
                    Id = book.Id,
                    Name = book.Title,
                    Author = book.Author,
                    DateOfBookCreation = book.DateOfBookCreation,
                    Description = book.Description,
                }).AsQueryable();

                viewModel.Books = bookDTOs;

                searchCategory = "";
            }

            viewModel.searchCategory = searchCategory;
            viewModel.inputValue = inputValue;
            return viewModel;
        }

        public async Task<bool> RateBookHelper(int stars, Guid bookId, ApplicationUser user)
        {
            try
            {
                var existingItem = _starRatingService.IQueryableGetAllAsync().Where(x=>x.User.Id==user.Id&&x.Book.Id==bookId).FirstOrDefault();
                if (existingItem!=null)
                {
                    existingItem.StarCount = stars;
                    await _starRatingService.UpdateAsync(existingItem);
                }
                else
                {
                    var entity = new StarRating();
                    entity.StarCount = stars;
                    entity.User = user;
                    entity.Book = await _bookService.GetByIdAsync(bookId);
                    await _starRatingService.AddAsync(entity);
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }

        public async Task<Document> GetDocumentPageEntity(Guid id)
        {
            return await _documentsService.GetByIdAsync(id);
        }
        #endregion

        #region UserFeedbackHelper
        public async Task<bool> SubmitUserFeedbackHelper(UserFeedbackDTO userFeedback, ApplicationUser user)
        {
            try
            {
                var emailBody = $@"
                    <table width='100%' border='0' cellspacing='0'cellpadding='8'>
                      <tr bgcolor='#f2f2f2'>
                        <th colspan='2' >Contact Form Submission</th>
                      </tr>
                      <tr>
                        <td width='30%'>Изпратено от:</td>
                        <td>{user.Email}</td>
                      </tr>
                      <tr>
                        <td width='30%'>Съобщение:</td>
                        <td>{userFeedback.Message}</td>
                      </tr>
                    </table>";

                var subjectText = "Имейл до Наука!";

                _emailSenderService.SendEmail(user!.Email!, emailBody, subjectText);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }
        #endregion

        #region ProgressBarHelper
        public ProgressBarSettings ProgressBarInformationFiller(ApplicationUser user)
        {
            var model = new ProgressBarSettings();

            model.UserAmount = user.Points;
            var alignedMembership = _membershipService.GetMembershipByPoints(user.Points);
            if (alignedMembership != null && alignedMembership.Id != Guid.Empty)
            {
                model.ProgressStart = alignedMembership.StartingNeededAmountOfPoints;
                model.ProgressEnd = alignedMembership.EndAmountOfPoints;
                model.MembershipName = alignedMembership.MembershipName;
            }
            else
            {
                model.ProgressStart = 0;
                model.ProgressEnd = 100;
                model.MembershipName = "Начален";
            }

            return model;
        }
        #endregion
    }
}
