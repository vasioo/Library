using Library.DataAccess.MainModels;
using Library.Models;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        #region Fields&Constructor
        private readonly IHomeControllerHelper _helper;
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
        public async Task<IActionResult> BookPage(int bookId)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var bookPageViewModel = _helper.GetBookPageAttributes(user, bookId);

            return View("~/Views/Home/BookPage.cshtml", bookPageViewModel);
        }
        #endregion

        #region Borrowed
        public async Task<IActionResult> Borrowed()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBorrowedPageAttributes(user!);

            return View("~/Views/Home/Borrowed.cshtml");
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
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var notificationSpecificForUser = _helper.GetNotificationsOfTheCurrentUser(user!);

            return View("~/Views/Home/Notifications.cshtml", notificationSpecificForUser);
        }
        #endregion
    }
}