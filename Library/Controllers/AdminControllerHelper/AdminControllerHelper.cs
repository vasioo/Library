using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.ViewModels;
using Library.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Web.Controllers.AdminControllerHelper
{
    public class AdminControllerHelper : IAdminControllerHelper
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly IUserLeasedBookService _userLeasedBookService;
        private readonly IMembershipService _membershipService;

        public AdminControllerHelper(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IUserLeasedBookService userLeasedBookService, IMembershipService membershipService)
        {
            _userManager = userManager;
            _userLeasedBookService = userLeasedBookService;
            _membershipService = membershipService;

        }

        public IQueryable<Membership> GetMemberships()
        {
            return _membershipService.IQueryableGetAllAsync();
        }

        public async Task<StatisticsViewModel> StatisticsHelper()
        {
            var viewModel = new StatisticsViewModel();

            viewModel.AmountOfUsers = _userManager.Users.Count();
            var workerRoles = new[] { "Admin", "Worker" };
            viewModel.AmountOfWorkers = await _userManager.Users
                    .ToListAsync()  // Retrieve all users from the database
                    .ContinueWith(users =>
                        users.Result
                        .Where(u => workerRoles.Any(role => _userManager.IsInRoleAsync(u, role).Result))
                        .Count()
                        );
            viewModel.MostLeasedBook = await _userLeasedBookService.MostLeasedBook();
            viewModel.MostReadGenres = await _userLeasedBookService.MostReadGenres();

            return viewModel;
        }

        public async Task<string> AddMembershipHelper(string name,int startPoints,int endPoints)
        {
            var membership = new Membership();

            var concurrentMembership = _membershipService.IQueryableGetAllAsync().Where(x => x.EndAmountOfPoints >= endPoints&&x.StartingNeededAmountOfPoints<= startPoints).FirstOrDefault();
            if (concurrentMembership!=null)
            {
                if (concurrentMembership.Id != Guid.Empty)
                {
                    return $"Не може да бъде добавено членство с точки между {startPoints} - {endPoints}";
                }
            }

            membership.MembershipName = name;
            membership.EndAmountOfPoints = endPoints;
            var res = await _membershipService.AddAsync(membership);
            if (res!=Guid.Empty)
            {
                return "confirmed";
            }
            return "";
        }

        public async Task<string> EditMembershipHelper(Guid id,string name, int startPoints, int endPoints)
        {
            var membership =await  _membershipService.GetByIdAsync(id);

            var concurrentMembership = _membershipService.IQueryableGetAllAsync().Where(x => x.EndAmountOfPoints >= endPoints && x.StartingNeededAmountOfPoints <= startPoints).FirstOrDefault();
            if (concurrentMembership != null)
            {
                if (concurrentMembership.Id != Guid.Empty&&concurrentMembership.Id!=id)
                {
                    return $"Не може да бъде добавено членство с точки между {startPoints} - {endPoints}";
                }
            }

            membership.MembershipName = name;
            membership.EndAmountOfPoints = endPoints;
            membership.StartingNeededAmountOfPoints = startPoints;
            var res = await _membershipService.UpdateAsync(membership);
            return "";
        }

        public async Task DeleteMembershipHelper(Guid id)
        {
            await _membershipService.RemoveAsync(id);
        }
    }
}
