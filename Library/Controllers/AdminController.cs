using Library.DataAccess.MainModels;
using Library.Models.ViewModels;
using Library.Web.Controllers.AdminControllerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
