using Library.DataAccess.MainModels;
using Library.Models.ViewModels;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        #region StaffManagement
        [Authorize(Roles = "Admin")]
        public IActionResult StaffManagement(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var users = _userManager.Users;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(usr => usr.LastName.Contains(searchString) || usr.FirstName.Contains(searchString));
            }
            return View("~/Views/Admin/StaffManagement.cshtml",users);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddAStaffMember()
        {
            return View("~/Views/Admin/AddAStaffMember.cshtml");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditStaffInformation(string userId)
        {
            var user =await _userManager.FindByIdAsync(userId);

            return View("~/Views/Admin/EditStaffPersonInformation.cshtml",user);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditInfo(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
            return View(StaffManagement(""));
        }
        #endregion

        #region Statistics
        [Authorize(Roles = "Admin")]
        public IActionResult Statistics()
        {
            var viewModel = new StatisticsViewModel();
            return View("~/Views/Admin/StaffManagement.cshtml",viewModel);
        }
        #endregion

        #region ClientManagement
        [Authorize(Roles = "Admin")]
        public IActionResult ClientManagement()
        {
            return View("~/Views/Admin/StaffManagement.cshtml");
        }
        #endregion
    }
}
