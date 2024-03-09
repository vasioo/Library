using Library.DataAccess.MainModels;
using Library.Models.Pagination;
using Library.Web.Controllers.AdminControllerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return View(ClientManagement("","All", 1));
        }
        #endregion

        #region Statistics
        public async Task<IActionResult> Statistics()
        {
            var viewModel = await _helper.StatisticsHelper();
            return View("~/Views/Admin/Statistics.cshtml", viewModel);
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
            catch (DbException ex)
            {
                return Json(new { status = false, Message = "Проблем в базата. Свържете се с отдел ИТ." });
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
            catch (DbException ex)
            {
                return Json(new { status = false, Message = "Проблем в базата. Свържете се с отдел ИТ." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Message = "Проблем в сървъра. Свържете се с отдел ИТ." });
            }
        }

        public async Task<JsonResult> DeleteMembership(Guid id,bool upper)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return Json(new { status = false, Message = "Невалидни параметри." });
                }
                await _helper.DeleteMembershipHelper(id,upper);
                return Json(new { status = true, Message = "Вашето членство беше изтрито." });
            }
            catch (DbException ex)
            {
                return Json(new { status = false, Message = "Проблем в базата. Свържете се с отдел ИТ." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Message = "Проблем в сървъра. Свържете се с отдел ИТ." });
            }
        }

        #endregion
    }
}
