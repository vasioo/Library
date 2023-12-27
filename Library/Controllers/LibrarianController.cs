using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class LibrarianController:Controller
    {
        private readonly ILibrarianControllerHelper _helper;
        UserManager<ApplicationUser> _userManager;

        public LibrarianController(ILibrarianControllerHelper helper, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _helper = helper;
        }

        #region Books
        //[Authorize(Roles = "Worker,Admin,SuperAdmin")]
        public async Task<IActionResult> AllBooksInformation()
        {
            //implement
            return View("~/Views/Admin/AllBooksInformation.cshtml");
        }
        public async Task<IActionResult> AddABook(ApplicationUser user)
        {
            //implement
            await _userManager.UpdateAsync(user);
            return View(AllBooksInformation());
        }
        public async Task<IActionResult> RemoveABook(Book book)
        {
            //implement
            return View(AllBooksInformation());
        }
        #endregion

    }
}
