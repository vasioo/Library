using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.ViewModels;
using Library.Web.Controllers.AdminControllerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace Library.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditStaffInformation(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return View("~/Views/Admin/EditStaffPersonInformation.cshtml", user);
        }
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditInfo(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
            return View(ClientManagement("", false));
        }
        #endregion

        #region Statistics
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Statistics()
        {
            var viewModel = await _helper.StatisticsHelper();
            return View("~/Views/Admin/Statistics.cshtml", viewModel);
        }
        #endregion

        #region ClientManagement
        //[Authorize(Roles = "Admin")]
        public IActionResult ClientManagement(string searchString, bool workersOnly)
        {
            ViewData["CurrentFilter"] = searchString;
            var users = _userManager.Users;
            if (workersOnly)
            {
                var workerRoles = new[] { "Admin", "Worker" };
                users = users.Where(u => workerRoles.Any(role => _userManager.IsInRoleAsync(u, role).Result));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(usr => usr.LastName.Contains(searchString) || usr.FirstName.Contains(searchString));
            }
            return View("~/Views/Admin/ClientManagement.cshtml", users);
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
