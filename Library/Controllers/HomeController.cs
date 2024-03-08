using Library.DataAccess.MainModels;
using Library.Models;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        #region Fields&Constructor
        private readonly IHomeControllerHelper _helper;
        private const string OpenLibraryApiBaseUrl = "https://openlibrary.org/api/books";
        UserManager<ApplicationUser> _userManager;

        public HomeController(IHomeControllerHelper helper, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _helper = helper;
        }
        #endregion

        #region MainPage

        public async Task<IActionResult> MainPage()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetMainPageAttributes(user!);

            return View("~/Views/Home/MainPage.cshtml", viewModel);
        }

        #endregion

        #region BookCollectionShower
        public async Task<IActionResult> BookCollectionShower()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBookCollectionAttributes(user!);

            return View("~/Views/Home/BookCollectionShower.cshtml", viewModel);
        }
        #endregion

        #region BookShower

        public async Task<IActionResult> BookShower(string category)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBooksAttributes(user!, category);

            return View("~/Views/Home/BookShower.cshtml", viewModel);
        }
        #endregion

        #region BookPage
        //[Authorize]
        public async Task<IActionResult> BookPage(Guid bookId)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var bookPageViewModel = await _helper.GetBookPageAttributes(user!, bookId);

            return View("~/Views/Home/BookPage.cshtml", bookPageViewModel);
        }

        [HttpPost]
        public async Task<JsonResult> BorrowBook(Guid bookId)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    if (!await _helper.BorrowBookPostHelper(bookId, user!))
                    {
                        return Json(new { status = false, Message = "The book could't be borrowed!" });
                    }
                }
                else
                {
                    return Json(new { status = false, Message = "There isn't a user with that username!" });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Error Conflicted" });
            }
            return Json(new { status = true, Message = "The book was borrowed successfully" });
        }

        [HttpPost]
        public async Task<JsonResult> UnborrowBook(Guid bookId)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return Json(new { status = false, Message = "There is no such user" });

                }
                if (!await _helper.UnborrowBookPostHelper(bookId, user!.Id))
                {
                    return Json(new { status = false, Message = "The book could't be removed!" });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Error Conflicted" });
            }
            return Json(new { status = true, Message = "The book was removed from borrowed successfully" });
        }

        [HttpPost]
        public async Task<JsonResult> ReadBook(string isbn)
        {
            string apiUrl = $"{OpenLibraryApiBaseUrl}?bibkeys=ISBN:{isbn}&format=json&jscmd=viewapi";
            try
            {
                if (isbn != null && isbn != "")
                {
                    using (var httpClient = new HttpClient())
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                            if (jsonObject.ContainsKey($"ISBN:{isbn}"))
                            {
                                var bookData = jsonObject[$"ISBN:{isbn}"];

                                if (bookData.preview_url != null)
                                {
                                    return Json(new
                                    {
                                        status = true,
                                        Message = bookData.preview_url.ToString()
                                    });
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        status = false,
                                        Message = "Няма такава книга в базата данни на openlibrary.com!"
                                    });
                                }
                            }
                        }
                        else
                        {
                            return Json(new { status = false, Message = "Няма такава книга в базата данни на openlibrary.com!" });
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка" });
            }
            return Json(new { status = false, Message = "Тази книга няма ISBN номер!" });

        }

        [HttpPost]
        public async Task<JsonResult> RateBook(int stars,Guid bookId)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return Json(new { status = false, Message = "Трябва да сте вписан в приложението като потребител." });
                }
                if (!await _helper.RateBookHelper(stars, bookId, user))
                {
                    throw new Exception(); 
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка." });
            }
            return Json(new { status = true, Message = "Вашият рейтинг беше записан успешно. Благодарим за отзивчивостта Ви!" });
        }
        #endregion

        #region Borrowed
        public async Task<IActionResult> Borrowed()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBorrowedPageAttributes(user!);

            return View("~/Views/Home/Borrowed.cshtml", viewModel);
        }
        #endregion

        #region Helpers

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

        #region Notifications
        public async Task<IActionResult> Notifications()
        {

            return View("~/Views/Home/Notifications.cshtml", _helper.GetNotifications());
        }
        #endregion

        #region Search

        public async Task<IActionResult> Search(string searchCategory, string inputValue, int page = 1)
        {
            var viewModel = await _helper.SearchViewModelHelper(searchCategory, inputValue, page);
            return View($"~/Views/Home/Search.cshtml", viewModel);
        }

        #endregion

        #region UserFeedback
        public IActionResult UserFeedback()
        {
            return View("~/Views/Home/UserFeedback.cshtml");
        }
        #endregion
    }
}