using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.ViewModels;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Packaging;
using System.Linq;

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

            return View("~/Views/Librarian/AllBooksInformation.cshtml", books);
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
        public async Task<JsonResult> ManageBookCategories(List<BookSubjectDTO> bookSubjectsDTO, List<BookCategoryDTO> bookCategoriesDTO)
        {
            try
            {
                string resultMessage = CheckAndPrintDuplicates(bookSubjectsDTO, bookCategoriesDTO);
                if (resultMessage.Trim() == null||resultMessage.Trim()=="")
                {
                    await _helper.AddBookSubjectAndCategoriesToDb(bookSubjectsDTO, bookCategoriesDTO);
                }
                else
                {
                    return Json(new { status = false, Message = resultMessage });
                }
            }
            catch (Exception)
            {
                return Json(new { status = true, Message = "Error Conflicted" });
            }
            return Json(new { status = true, Message = "The item was added successfully" });
        }

        public interface HelperBookDTO
        {
            string SubjectName { get; set; }
        }

        static string CheckAndPrintDuplicates(List<BookSubjectDTO> subjectDTOs, List<BookCategoryDTO> categoryDTOs)
        {
            var allNames = subjectDTOs.Select(dto => dto.SubjectName.ToLowerInvariant())
                                      .Concat(categoryDTOs.SelectMany(dto => new[] { dto.CategoryName, dto.SubjectName }
                                      .Select(name => name.ToLowerInvariant())))
                                      .Distinct();

            var duplicates = allNames.GroupBy(name => name)
                                     .Where(group => group.Count() > 1)
                                     .Select(group => group.Key)
                                     .ToList();

            if (duplicates.Any())
            {
                string message = "Duplicates found:\n";

                foreach (var duplicate in duplicates)
                {
                    List<HelperBookDTO> allEntities = new List<HelperBookDTO>();
                    allEntities.AddRange(subjectDTOs.Cast<HelperBookDTO>());
                    allEntities.AddRange(categoryDTOs.Cast<HelperBookDTO>());


                    var duplicateEntities = allEntities
                                           .Where(dto => dto.SubjectName.ToLowerInvariant() == duplicate 
                                           || (dto is BookCategoryDTO categoryDTO && categoryDTO.CategoryName.ToLowerInvariant() == duplicate))
                                           .ToList();

                    foreach (var entity in duplicateEntities)
                    {
                        entity.SubjectName = char.ToUpper(entity.SubjectName[0]) + entity.SubjectName.Substring(1);
                        if (entity is BookCategoryDTO categoryDTO)
                        {
                            categoryDTO.CategoryName = char.ToUpper(categoryDTO.CategoryName[0]) + categoryDTO.CategoryName.Substring(1);
                        }
                    }

                    message += $"Duplicate: {duplicate}\n";
                    foreach (var entity in duplicateEntities)
                    {
                        message += $"{entity.GetType().Name}: {entity.SubjectName} | {(entity is BookCategoryDTO ? ((BookCategoryDTO)entity).CategoryName : "")}\n";
                    }
                }

                return message;
            }
            else
            {
                return "";
            }
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
                await _helper.EditABook(book, imageObj);
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
