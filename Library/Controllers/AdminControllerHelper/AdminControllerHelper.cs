using Library.DataAccess.MainModels;
using Library.Models.ViewModels;
using Library.Services.Interfaces;

namespace Library.Web.Controllers.AdminControllerHelper
{
    public class AdminControllerHelper : IAdminControllerHelper
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly IUserLeasedBookService _userLeasedBookService;

        public AdminControllerHelper(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IUserLeasedBookService userLeasedBookService)
        {
            _userManager = userManager;
            _userLeasedBookService = userLeasedBookService;
        }

        public async Task<StatisticsViewModel> StatisticsHelper()
        {
            var viewModel = new StatisticsViewModel();

            viewModel.AmountOfUsers =_userManager.Users.Count();
            var workerRoles = new[] { "Admin", "Worker" };
            viewModel.AmountOfWorkers =_userManager.Users.Where(u => workerRoles.Any(role => _userManager.IsInRoleAsync(u, role).Result)).Count();
            viewModel.MostLeasedBook =await _userLeasedBookService.MostLeasedBook();
            viewModel.MostReadGenres =await _userLeasedBookService.MostReadGenres();

            return viewModel;
        }
    }
}
