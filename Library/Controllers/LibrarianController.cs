using Humanizer.Localisation;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.Pagination;
using Library.Models.ViewModels;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Library.Web.Controllers
{
    public class LibrarianController : Controller
    {
        #region FieldsAndConstructor
        private static readonly HttpClient client = new HttpClient();
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
                books = books.Where(book => book.Author.Contains(searchString) || book.Title.Contains(searchString)
                || book.DateOfBookCreation.ToString().Contains(searchString));
            }
            int pageSize = 30;
            var paginatedList = PaginatedList<Book>.CreateAsync(books.AsNoTracking(), page ?? 1, pageSize);
            return View("~/Views/Librarian/AllBooksInformation.cshtml", paginatedList);
        }

        public async Task<IActionResult> AddABook()
        {
            var viewModel = await _helper.AddABookHelper();

            return View("~/Views/Librarian/AddABook.cshtml", viewModel);
        }

        public async Task<IActionResult> AddABookISBN()
        {
            List<string> genreItems = await _helper.GetAllGenresHelper();
            return View("~/Views/Librarian/AddABookByISBN.cshtml", genreItems);
        }

        public async Task<IActionResult> ManageBookCategories()
        {
            var data = _helper.GetAllBookSubjects();

            return View("~/Views/Librarian/ManageBookCategories.cshtml", data);
        }

        public async Task<IActionResult> EditBookInformation(Guid bookId)
        {
            var viewModel = await _helper.EditABookHelper(bookId);

            return View("~/Views/Librarian/EditBookInformation.cshtml", viewModel);
        }

        #endregion

        #region BookActions

        [HttpPost]
        public async Task<JsonResult> AddABookPost(BookChangersViewModel book, string imageObj)
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

        [HttpGet]
        public async Task<JsonResult> FindBookByISBN(string isbn)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://openlibrary.org/isbn/{isbn}");
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();


                if (responseBody != null)
                {
                    return Json(new { status = true, Message = responseBody });
                }
                else
                {
                    return Json(new { status = false, Message = "Книгата не може да бъде намерена." });
                }
            }
            catch (HttpRequestException)
            {
                return Json(new { status = false, Message = "Книгата не може да бъде намерена." });
            }
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

        public async Task<JsonResult> EditABookPost(BookChangersViewModel book, string imageObj)
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

        public async Task<JsonResult> RemoveABook(Guid bookId)
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

        [HttpPost]
        public async Task<JsonResult> SaveBook(BookViewModelDTO viewModelDTO)
        {
            try
            {
                await _helper.SaveBookHelper(viewModelDTO);
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка с добавянето на книгата." });
            }
            return Json(new { status = true, Message = "Книгата беше добавена успешно." });
        }

        public async Task<IActionResult> LeasedTracker(string Category)
        {
            var viewModel = await _helper.GetLeasedTrackerData(Category);
            return View("~/Views/Librarian/LeasedTracker.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> LeaseBookOrNot(Guid userLeasedId, bool lease)
        {
            try
            {
                var res = await _helper.LeaseBookOrNotHelper(userLeasedId, lease);
                if (res)
                {
                    return Json(new { status = true, Message = "Промените бяха направени." });
                }
                else
                {
                    return Json(new { status = false, Message = "Възникна грешка." });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> StopLeasing(Guid userLeasedId)
        {
            try
            {
                var res = await _helper.StopLeasingHelper(userLeasedId);
                if (res)
                {
                    return Json(new { status = true, Message = "Промените бяха направени." });
                }
                else
                {
                    return Json(new { status = false, Message = "Възникна грешка." });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteUserLeasedEntity(Guid userLeasedId)
        {
            try
            {
                var res = await _helper.DeleteLeasedEntityHelper(userLeasedId);
                if (res)
                {
                    return Json(new { status = true, Message = "Промените бяха направени." });
                }
                else
                {
                    return Json(new { status = false, Message = "Възникна грешка." });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка." });
            }
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
            var viewModel = await _helper.GetReportPageModel();
            return View("~/Views/Librarian/Report.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> LoadBookInformation(DateTime startDate, DateTime endDate, int selectedCountOfItems)
        {
            try
            {
                var returnData = await _helper.GetBookInformationByTimeAndCount(startDate, endDate, selectedCountOfItems);
                return Json(new { status = true, Data = returnData });
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Error Conflicted" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> LoadGenreInformation(DateTime startDate, DateTime endDate, int selectedCountOfItems)
        {
            try
            {
                var returnData = await _helper.GetGenreInformationByTimeAndCount(startDate, endDate);
                return Json(new { status = true, Data = returnData });
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Error Conflicted" });
            }
        }

        public async Task<IActionResult> AddAReport(LibrarianReport report)
        {
            //implement
            return View(Report());
        }
        #endregion
    }
}
