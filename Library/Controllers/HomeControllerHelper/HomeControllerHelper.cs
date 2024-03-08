using Hangfire;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.ViewModels;
using Library.Services.Interfaces;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class HomeControllerHelper : IHomeControllerHelper
    {
        #region Fields&Constructor
        private readonly INotificationService _notificationService;
        private readonly IBookService _bookService;
        private readonly IBlogPostService _blogPostService;
        private readonly IBookCategoryService _bookCategoryService;
        private readonly IUserLeasedBookService _userLeasedBookService;
        private readonly IBookSubjectService _bookSubjectService;
        private readonly IStarRatingService _starRatingService;


        public HomeControllerHelper(INotificationService notificationService, IBookSubjectService bookSubjectService,
            IBookService bookService, IStarRatingService starRatingService, IBookCategoryService bookCategoryService, IUserLeasedBookService userLeasedBookService, IBlogPostService blogPostService)
        {
            _notificationService = notificationService;
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
            _userLeasedBookService = userLeasedBookService;
            _bookSubjectService = bookSubjectService;
            _blogPostService = blogPostService;
            _starRatingService = starRatingService;
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
            RecurringJob.AddOrUpdate(() => _notificationService.AddDailyNotification(), "0 14 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => _notificationService.AddWeeklyNotification(), "0 17 * * 0", TimeZoneInfo.Local);
            var viewModel = new MainPageViewModel();

            viewModel.BestSellers = _bookService.GetTop6BooksByCriteria(user, "");
            viewModel.RecommendedBooks = _bookService.GetTop6BooksByCriteria(user, "recommended");

            return viewModel;
        }
        #endregion

        #region NotificationsHelper
        public IQueryable<Notification> GetNotifications()
        {
            var notifications = _notificationService.IQueryableGetAllAsync();
            return notifications;
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
            if (user != null)
            {
                var borrowedBook = await _userLeasedBookService.GetBorrowedBookByUserIdAndBookId(bookId, user.Id);
                if (borrowedBook.Id != Guid.Empty)
                {
                    viewModel.HasUserBorrowedIt = true;
                    if (!borrowedBook.Approved&&!borrowedBook.IsRead)
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
            if (searchCategory == "Authors")
            {
                var authors = _blogPostService.IQueryableGetAllAsync()
                    .Where(x => x.IsForAuthor && x.Title.Contains(inputValue) || x.Content.Contains(inputValue))
                    .Skip((page - 1) * 20).Take(20);
                viewModel.TotalPages = (int)Math.Ceiling((double)_blogPostService.IQueryableGetAllAsync().Count() / 20);
                viewModel.PageNumber = page;
                var authorDTO = authors.Select(blogPost => new BlogPost
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
                    .Where(x => x.Title.Contains(inputValue))
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

        public async Task<bool> RateBookHelper(int stars, Guid bookId,ApplicationUser user)
        {
            try
            {
                var entity = new StarRating();
                entity.StarCount = stars;
                entity.User = user;
                entity.Book = await _bookService.GetByIdAsync(bookId);
                await _starRatingService.AddAsync(entity);
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
