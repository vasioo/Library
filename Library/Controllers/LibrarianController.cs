using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.Pagination;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Web.Controllers
{
    public class LibrarianController : Controller
    {
        #region FieldsAndConstructor
        private readonly ILibrarianControllerHelper _helper;
        UserManager<ApplicationUser> _userManager;

        public LibrarianController(ILibrarianControllerHelper helper, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _helper = helper;
        }
        #endregion

        #region Books
        //[Authorize(Roles = "Worker,Admin,SuperAdmin")]

        #region BookViews
        public async Task<IActionResult> AllBooksInformation(int? page, string searchString, string currentFilter)
        {
            ViewData["CurrentFilter"] = searchString;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var books = _helper.GetAllBooks();
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(book => book.Author.Contains(searchString) || book.Name.Contains(searchString)
                ||book.DateOfBookCreation.ToString().Contains(searchString)||book.AvailableItems.ToString().Contains(searchString));
            }
            int pageSize = 30;
            var paginatedList = PaginatedList<Book>.CreateAsync(books.AsNoTracking(), page ?? 1, pageSize);
            return View("~/Views/Librarian/AllBooksInformation.cshtml", paginatedList);
        }

        public IActionResult AddABook()
        {
            ViewBag.BookCategories = _helper.GetAllBookCategories().ToList();

            return View("~/Views/Librarian/AddABook.cshtml");
        }

        public async Task<IActionResult> ManageBookCategories()
        {
            ViewBag.BookCategories = _helper.GetAllBookCategories().ToList();

            var data = _helper.GetAllBookSubjects();

            return View("~/Views/Librarian/ManageBookCategories.cshtml", data);
        }

        public async Task<IActionResult> EditBookInformation(int bookId)
        {
            ViewBag.BookCategories = _helper.GetAllBookCategories().ToList();

            var book = await _helper.GetBook(bookId);

            return View("~/Views/Librarian/EditBookInformation.cshtml", book);
        }

        #endregion

        #region BookActions

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
                if (resultMessage.Trim() == null || resultMessage.Trim() == "")
                {
                    string str = await _helper.AddBookSubjectAndCategoriesToDb(bookSubjectsDTO, bookCategoriesDTO);

                    if (str != "" && str != null)
                    {
                        return Json(new { status = false, Message = str });
                    }
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

        #region BookHelpers

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

        #endregion

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
