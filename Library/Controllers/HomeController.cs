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
        private readonly IHomeControllerHelper _helper;
        UserManager<ApplicationUser> _userManager;

        public HomeController(IHomeControllerHelper helper, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _helper = helper;
        }

        #region MainPage

        public async Task<IActionResult> MainPage()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetMainPageAttributes(user!);

            return View("~/Views/Home/MainPage.cshtml",viewModel);
        }

        #endregion

        #region BookCollectionShower
        public async Task<IActionResult> BookCollectionShower()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBookCollectionAttributes(user!);

            return View("~/Views/Home/BookCollectionShower.cshtml",viewModel);
        }
        #endregion

        #region BookShower

        public async Task<IActionResult> BookShower()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBooksAttributes(user!);

            return View("~/Views/Home/BookShower.cshtml", viewModel);
        }

        #region BookPage
        public IActionResult BookPage()
        {
            //a single book and its specifications as well as the ability to borrow it
            return View("~/Views/Home/BookPage.cshtml");
        }
        #endregion

        #region Borrowed
        public IActionResult Borrowed()
        {
            //which books have been borrowed by the user 
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