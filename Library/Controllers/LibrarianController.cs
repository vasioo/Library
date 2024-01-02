using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.ViewModels;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Library.Web.Controllers
{
    public class LibrarianController : Controller
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
            var books = _helper.GetAllBooks();

            return View("~/Views/Librarian/AllBooksInformation.cshtml",books);
        }

        public IActionResult AddABook()
        {
            ViewBag.BookCategories = _helper.GetAllBookCategories().ToList();

            return View("~/Views/Librarian/AddABook.cshtml");
        }
        [HttpPost]
        public async Task<JsonResult> AddABookPost(BookDTO book, string imageObj)
        {
            try
            {
                await _helper.AddABookToDatabase(book, imageObj);
            }
            catch (Exception)
            {
                return Json(new { status = true, Message = "Error Conflicted" });
            }
            return Json(new { status = true, Message = "The item was added successfully" });
        }

        [HttpPost]
        public async Task<JsonResult> AddABookCategory(string categoryName)
        {
            try
            {
                await _helper.AddABookCategoryToDatabase(categoryName);
            }
            catch (Exception)
            {
                return Json(new { status = true, Message = "Error Conflicted" });
            }
            return Json(new { status = true, Message = "The item was added successfully" });
        }

        public async Task<IActionResult> EditBookInformation(int bookId)
        {
            ViewBag.BookCategories = _helper.GetAllBookCategories().ToList();

            var book = await _helper.GetBook(bookId);

            return View("~/Views/Librarian/EditBookInformation.cshtml", book);
        }

        public async Task<JsonResult> EditABookPost(BookDTO book, string imageObj) 
        {
            //implement
            try
            {
                await _helper.EditABook(book,imageObj);
            }
            catch (Exception)
            {
                return Json(new { status = true, Message = "Error Conflicted" });
            }
            return Json(new { status = true, Message = "The item was added successfully" });
        }

        public async Task<IActionResult> ManageBookCategories()
        {
            ViewBag.BookCategories = _helper.GetAllBookCategories().ToList();

            return View("~/Views/Librarian/ManageBookCategories.cshtml");
        }

        public async Task<JsonResult> RemoveABook(int bookId)
        {
            try
            {
                await _helper.RemoveABook(bookId);
            }
            catch (Exception)
            {
                return Json(new { status = true, Message = "Error Conflicted" });
            }
            return Json(new { status = true, Message = "The item was removed successfully" });
        }
        #endregion

        #region Reports
        public async Task<IActionResult> Report()
        {
            //implement(could be to user or to admin)
            return View("~/Views/Librarian/Report.cshtml");
        }

        public async Task<IActionResult> AddAReport(LibrarianReport report)
        {
            //implement
            return View(Report());
        }
        #endregion
    }
}
