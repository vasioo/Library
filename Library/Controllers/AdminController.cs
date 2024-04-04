using Library.DataAccess.MainModels;
using Library.Models.DTO;
using Library.Models.Pagination;
using Library.Web.Controllers.AdminControllerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Macs;
using System.Data.Common;

namespace Library.Web.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        #region Constructor
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly IAdminControllerHelper _helper;

        public AdminController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IAdminControllerHelper helper)
        {
            _userManager = userManager;
            _helper = helper;
        }
        #endregion
        
        #region StaffManagement

        public async Task<IActionResult> EditStaffInformation(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return View("~/Views/Admin/EditStaffPersonInformation.cshtml", user);
        }

        public async Task<IActionResult> EditInfo(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
            return View(ClientManagement("", "All", 1));
        }

        #endregion

        #region Reports

        public async Task<IActionResult> Report()
        {
            var viewModel = await _helper.GetReportPageModel();
            return View("~/Views/Admin/Report.cshtml", viewModel);
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
                return Json(new { status = false, Message = "Възникна грешка" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> LoadGenreInformation(DateTime startDate, DateTime endDate)
        {
            try
            {
                var returnData = await _helper.GetGenreInformationByTimeAndCount(startDate, endDate);
                return Json(new { status = true, Data = returnData });
            }
            catch (Exception)
            {
                return Json(new { status = false, Message = "Възникна грешка" });
            }
        }

        #endregion

        #region ClientManagement

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ClientManagement(string searchString, string roleFilter, int? page)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["RoleFilter"] = roleFilter ?? "All";

            var users = _userManager.Users;

            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All")
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleFilter);
                users = users.Where(u => usersInRole.Contains(u));
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                string searchStringLower = searchString.ToLower();

                users = users.Where(usr =>
                    usr.Email.ToLower().Contains(searchStringLower) ||
                    usr.UserName.ToLower().Contains(searchStringLower)
                );
            }



            if (searchString != null)
            {
                page = 1;
            }

            int pageSize = 30;
            var paginatedList = PaginatedList<ApplicationUser>.CreateAsync(users.AsNoTracking(), page ?? 1, pageSize);
            return View("~/Views/Admin/ClientManagement.cshtml", paginatedList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditInfo(EditInfoDTO userInfo)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userInfo.Id);
                if (user != null)
                {
                    user.Salary = userInfo.Salary;
                    user.Position = userInfo.Position;
                    if (userInfo.Position == "Администратор")
                    {
                        var rolesList = new List<string>()
                        {
                            "Admin","Worker"
                        };
                        await _userManager.RemoveFromRolesAsync(user, rolesList);
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else if (userInfo.Position == "Библиотекар")
                    {
                        var rolesList = new List<string>()
                        {
                            "Admin","Worker"
                        };
                        await _userManager.RemoveFromRolesAsync(user, rolesList);
                        await _userManager.AddToRoleAsync(user, "Worker");
                    }
                    else
                    {
                        var rolesList = new List<string>()
                        {
                            "Admin","Worker"
                        };
                        await _userManager.RemoveFromRolesAsync(user, rolesList);
                    }
                    await _userManager.UpdateAsync(user);
                    return Json(new { status = true, message = "Данните бяха записани успешно!" });
                }
            }
            return Json(new { status = false, errors = "Възникна грешка" });
        }

        [HttpPost]
        public async Task<JsonResult> BanUser(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "SuperAdmin") || await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return Json(new { status = false, errors = "Потребителя не може да бъде блокиран, заради ролята му." });
                    }
                    user.BanStatus = "";
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    return Json(new { status = false, errors = "Потребителския имейл не съществува в базата!" });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, errors = "Възникна грешка." });
            }
            return Json(new { status = true, errors = "Потребителя беше блокиран в приложението." });
        }
        [HttpPost]
        public async Task<JsonResult> UnbanUser(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    user.BanStatus = "";
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    return Json(new { status = false, errors = "Потребителския имейл не съществува в базата!" });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, errors = "Възникна грешка." });
            }
            return Json(new { status = true, errors = "Потребителя беше отблокиран от приложението." });
        }
        #endregion

        #region BookCategories
        public IActionResult ManageCategories()
        {
            return View("~/Views/Admin/ManageCategories.cshtml");
        }

        #endregion

        #region ManageMemberships

        public IActionResult ManageMemberships()
        {
            var data = _helper.GetMemberships();
            return View("~/Views/Admin/ManageMemberships.cshtml", data);
        }

        public async Task<JsonResult> AddMembership(string addMembershipName, int addStarterNeededPoints, int addNeededAmountOfPoints)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addMembershipName) || addStarterNeededPoints < 0 || addNeededAmountOfPoints < 0 || addNeededAmountOfPoints <= addStarterNeededPoints)
                {
                    return Json(new { status = false, Message = "Невалидни параметри." });
                }

                var result = await _helper.AddMembershipHelper(addMembershipName.Trim(), addStarterNeededPoints, addNeededAmountOfPoints);
                if (result == "confirmed")
                {
                    return Json(new { status = true, Message = "Вашето ново членство беше записано." });
                }
                else if (!string.IsNullOrEmpty(result))
                {
                    return Json(new { status = false, Message = result });
                }
                return Json(new { status = false, Message = "Проблем в сървъра. Свържете се с отдел ИТ." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Message = "Проблем в сървъра. Свържете се с отдел ИТ." });
            }
        }

        public async Task<JsonResult> EditMembership(string membershipId, string membershipName, int starterNeededPoints, int neededAmountOfPoints)
        {
            try
            {
                var membershipIdGUID = Guid.Parse(membershipId);
                if (membershipIdGUID == Guid.Empty || string.IsNullOrWhiteSpace(membershipName) || starterNeededPoints < 0 || neededAmountOfPoints < 0 || neededAmountOfPoints <= starterNeededPoints)
                {
                    return Json(new { status = false, Message = "Невалидни параметри." });
                }

                var result = await _helper.EditMembershipHelper(membershipIdGUID, membershipName, starterNeededPoints, neededAmountOfPoints);
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { status = true, Message = "Вашето членство беше променено." });
                }
                else if (!string.IsNullOrEmpty(result))
                {
                    return Json(new { status = false, Message = result });
                }
                return Json(new { status = false, Message = "Проблем в сървъра. Свържете се с отдел ИТ." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Message = "Проблем в сървъра. Свържете се с отдел ИТ." });
            }
        }

        public async Task<JsonResult> DeleteMembership(Guid id, bool upper)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return Json(new { status = false, Message = "Невалидни параметри." });
                }
                await _helper.DeleteMembershipHelper(id, upper);
                return Json(new { status = true, Message = "Вашето членство беше изтрито." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Message = "Проблем в сървъра. Свържете се с отдел ИТ." });
            }
        }

        #endregion
    }
}
