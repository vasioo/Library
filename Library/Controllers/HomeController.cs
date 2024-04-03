using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        #region Fields&Constructor
        private readonly IHomeControllerHelper _helper;
        private const string OpenLibraryApiBaseUrl = "https://openlibrary.org/api/books";
        private readonly UserManager<ApplicationUser> _userManager;

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

            if (user != null)
            {
                viewModel.ProgressBarSettings = _helper.ProgressBarInformationFiller(user!);
            }
            return View("~/Views/Home/MainPage.cshtml", viewModel);
        }

        #endregion

        #region BookCollectionShower

        public async Task<IActionResult> BookCollectionShower()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBookCollectionAttributes(user!);
            if (user != null)
            {
                viewModel.ProgressBarSettings = _helper.ProgressBarInformationFiller(user!);
            }

            return View("~/Views/Home/BookCollectionShower.cshtml", viewModel);
        }

        #endregion

        #region BookShower
        [Authorize]
        public async Task<IActionResult> BookShower(string category)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBooksAttributes(user!, category);

            if (user != null)
            {
                viewModel.ProgressBarSettings = _helper.ProgressBarInformationFiller(user!);
            }

            return View("~/Views/Home/BookShower.cshtml", viewModel);
        }
        #endregion

        #region BookPage
        [Authorize]
        public async Task<IActionResult> BookPage(Guid bookId)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = await _helper.GetBookPageAttributes(user!, bookId);
            if (user != null)
            {
                viewModel.ProgressBarSettings = _helper.ProgressBarInformationFiller(user!);
            }
            return View("~/Views/Home/BookPage.cshtml", viewModel);
        }

        [HttpPost]
        [Authorize]
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
                        return Json(new { status = false, Message = "Книгата не може да бъде отдавана!" });
                    }
                }
                else
                {
                    return Json(new { status = false, Message = "За да четете, трябва да се логнати в приложението!" });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка" });
            }
            return Json(new { status = true, Message = "Книгата беше отдадена успешно" });
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> ReadBook(string isbn)
        {
            string apiUrl = $"{OpenLibraryApiBaseUrl}?bibkeys=ISBN:{isbn}&format=json&jscmd=viewapi";
            try
            {
                if (isbn != null && isbn != "")
                {
                    var username = HttpContext.User?.Identity?.Name ?? "";
                    var user = await _userManager.FindByNameAsync(username);
                    if (user != null)
                    {
                        var isNew = await _helper.UpdateUsersReadAttribute(user, isbn);
                        if (!String.IsNullOrEmpty(isNew))
                        {
                            return Json(new
                            {
                                status = true,
                                Message = isNew
                            });
                        }
                        else
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
                                            await _helper.UpdateBookLink(isbn, bookData.preview_url.ToString());
                                            user.Points += 5;
                                            await _userManager.UpdateAsync(user);
                                            return Json(new
                                            {
                                                status = true,
                                                Message = bookData.preview_url.ToString(),
                                                FirstUserRead = "1"
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
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка" });
            }
            return Json(new { status = false, Message = "Тази книга няма ISBN номер!" });

        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> RateBook(int stars, Guid bookId)
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

        [Authorize]
        public async Task<IActionResult> Borrowed()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = _helper.GetBorrowedPageAttributes(user!);
            if (user != null)
            {
                viewModel.ProgressBarSettings = _helper.ProgressBarInformationFiller(user!);
            }
            return View("~/Views/Home/Borrowed.cshtml", viewModel);
        }

        #endregion

        #region Helpers
        [HttpGet("/Home/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? errorCode = null)
        {
            var errorMessage = GetHttpStatusMessage(errorCode ?? HttpContext.Response.StatusCode);

            var customErrorModel = new CustomModelError
            {
                StatusCode = errorCode ?? HttpContext.Response.StatusCode,
                CustomErrorMessage = errorMessage
            };
            return View("~/Views/Shared/Error.cshtml", customErrorModel);
        }

        private string GetHttpStatusMessage(int statusCode)
        {
            return statusCode switch
            {
                (int)HttpStatusCode.BadRequest => "Лоша заявка",
                (int)HttpStatusCode.Unauthorized => "Неоторизиран вход",
                (int)HttpStatusCode.Forbidden => "Забранено",
                (int)HttpStatusCode.NotFound => "Не е намерено",
                (int)HttpStatusCode.InternalServerError => "Сървърна грешка",
                (int)HttpStatusCode.NotImplemented => "Сървърна грешка",
                (int)HttpStatusCode.BadGateway => "Лош изход",
                (int)HttpStatusCode.ServiceUnavailable => "Недостъпно",
                (int)HttpStatusCode.LoopDetected => "Засечен цикъл",
                _ => "Грешка"
            };
        }
        #endregion

        #region Search

        public async Task<IActionResult> Search(string searchCategory, string inputValue, int page = 1)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);
            var viewModel = await _helper.SearchViewModelHelper(searchCategory, inputValue, page);
            if (user != null)
            {
                viewModel.ProgressBarSettings = _helper.ProgressBarInformationFiller(user!);
            }
            return View($"~/Views/Home/Search.cshtml", viewModel);
        }

        public async Task<IActionResult> DocumentPage(Guid id)
        {
            var model = await _helper.GetDocumentPageEntity(id);
            return View("~/Views/Home/DocumentPage.cshtml", model);
        }

        #endregion

        #region UserFeedback

        [Authorize]
        public IActionResult UserFeedback()
        {
            return View("~/Views/Home/UserFeedback.cshtml");
        }

        [Authorize]
        public async Task<JsonResult> SubmitUserFeedback(UserFeedbackDTO userFeedback)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    if (user.Email != userFeedback.Email)
                    {
                        return Json(new { status = false, Message = "Невъзможно е изпращането, тъй като предоставения имейл е различен от активния в приложението!" });
                    }
                    if (!await _helper.SubmitUserFeedbackHelper(userFeedback, user))
                    {
                        return Json(new { status = false, Message = "Проблем с изпращането, опитайте отново!" });
                    }
                }
                else
                {
                    return Json(new { status = false, Message = "Трябва да сте логнати в приложението!" });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка" });
            }
            return Json(new { status = true, Message = "Имейлът беше изпратен успешно!" });
        }
        #endregion
    }
}