using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Models.DTO;
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
        private readonly IBookService _bookService;

        public AdminControllerHelper(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IUserLeasedBookService userLeasedBookService, IMembershipService membershipService, IBookService bookService)
        {
            _userManager = userManager;
            _userLeasedBookService = userLeasedBookService;
            _membershipService = membershipService;
            _bookService = bookService;
        }
        
        public async Task<StatisticsViewModel> StatisticsHelper()
        {
            var viewModel = new StatisticsViewModel();

            viewModel.AmountOfUsers = _userManager.Users.Count();
            var workerRoles = new[] { "Admin", "Worker" };
            viewModel.AmountOfWorkers = await _userManager.Users
                    .ToListAsync() 
                    .ContinueWith(users =>
                        users.Result
                        .Where(u => workerRoles.Any(role => _userManager.IsInRoleAsync(u, role).Result))
                        .Count()
                        );
            var leasedBook = await _userLeasedBookService.GetBooksInformationByTimeAndCountOfItems(DateTime.Now.AddHours(-24), DateTime.Now, 1);

            foreach (var item in leasedBook)
            {
                var mostLeasedBook = new ReportBookDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                viewModel.MostLeasedBook = mostLeasedBook;
            }
            viewModel.MostReadGenres = await _userLeasedBookService.MostReadGenres(DateTime.Now.AddHours(-24), DateTime.Now);

            return viewModel;
        }

        public async Task<IEnumerable<ReportBookDTO>> GetBookInformationByTimeAndCount(DateTime startDate, DateTime endDate, int selectedCountOfItems)
        {
            return await _userLeasedBookService.GetBooksInformationByTimeAndCountOfItems(startDate, endDate, selectedCountOfItems);
        }

        public async Task<List<string>> GetGenreInformationByTimeAndCount(DateTime startDate, DateTime endDate)
        {
            return await _userLeasedBookService.MostReadGenres(startDate, endDate);
        }


        #region MembershipHelpers
        public IQueryable<Membership> GetMemberships()
        {
            return _membershipService.IQueryableGetAllAsync();
        }

        public async Task<string> AddMembershipHelper(string name, int startPoints, int endPoints)
        {
            var membership = new Membership();

            var namememb = _membershipService.IQueryableGetAllAsync().Where(x => x.MembershipName == name).FirstOrDefault();
            if (namememb != null)
            {
                if (namememb.Id != Guid.Empty)
                {
                    return $"Не може да има още едно членство с името - {name}";
                }
            }
            var concurrentMembership = _membershipService.IQueryableGetAllAsync()
                         .Where(x => x.EndAmountOfPoints >= endPoints && x.StartingNeededAmountOfPoints <= startPoints).FirstOrDefault();
            if (concurrentMembership != null)
            {
                if (concurrentMembership.Id != Guid.Empty)
                {
                    return $"Не може да бъде добавено членство с точки между {startPoints} - {endPoints}," +
                        $" защото членството {concurrentMembership.MembershipName} е цялото в този диапазон. Променете го!";
                }
            }
            var prev = _membershipService.IQueryableGetAllAsync().Where(x => x.StartingNeededAmountOfPoints < startPoints).OrderByDescending(x => x.StartingNeededAmountOfPoints).FirstOrDefault();
            var next = _membershipService.IQueryableGetAllAsync().Where(x => x.EndAmountOfPoints > endPoints).OrderBy(x => x.EndAmountOfPoints).FirstOrDefault();
            if (prev != null)
            {
                if (prev.Id != Guid.Empty)
                {
                    if (prev.EndAmountOfPoints > startPoints)
                    {
                        if (startPoints - 1 > prev.StartingNeededAmountOfPoints)
                        {
                            prev.EndAmountOfPoints = startPoints - 1;
                            await _membershipService.UpdateAsync(prev);
                        }
                        else
                        {
                            return $"Елементът не може да бъде добавен, защото в членството ( {prev.MembershipName} )" +
                              $" крайните точки стават по-малки/равни на началните точки. Променете го!";
                        }
                    }
                }

            }
            if (next != null)
            {
                if (next.Id != Guid.Empty)
                {
                    if (next.StartingNeededAmountOfPoints < endPoints)
                    {
                        if (endPoints + 1 < next.EndAmountOfPoints)
                        {
                            next.StartingNeededAmountOfPoints = endPoints + 1;
                            await _membershipService.UpdateAsync(next);
                        }
                        else
                        {
                            return $"Елементът не може да бъде добавен, защото в членството ( {next.MembershipName} )" +
                                $" началните точки стават по-големи/равни на крайните точки. Променете го!";
                        }
                    }
                }
            }
            membership.MembershipName = name;
            membership.EndAmountOfPoints = endPoints;
            membership.StartingNeededAmountOfPoints = startPoints;
            var res = await _membershipService.AddAsync(membership);
            if (res != Guid.Empty)
            {
                return "confirmed";
            }
            return "";
        }

        public async Task<string> EditMembershipHelper(Guid id, string name, int startPoints, int endPoints)
        {
            var membership = await _membershipService.GetByIdAsync(id);

            var namememb = _membershipService.IQueryableGetAllAsync().Where(x => x.MembershipName == name && x.Id != id).FirstOrDefault();
            if (namememb != null)
            {
                if (namememb.Id != Guid.Empty)
                {
                    return $"Не може да има още едно членство с името - {name}";
                }
            }
            var concurrentMembership = _membershipService.IQueryableGetAllAsync()
                          .Where(x => x.EndAmountOfPoints >= endPoints && x.StartingNeededAmountOfPoints <= startPoints).FirstOrDefault();
            if (concurrentMembership != null)
            {
                if (concurrentMembership.Id != Guid.Empty)
                {
                    if (concurrentMembership.Id != id)
                    {
                        return $"Не може да бъде добавено членство с точки между {startPoints} - {endPoints}," +
                        $" защото членството {concurrentMembership.MembershipName} е цялото в този диапазон. Променете го!";
                    }
                }
            }
            var prev = _membershipService.IQueryableGetAllAsync().Where(x => x.StartingNeededAmountOfPoints < startPoints).OrderByDescending(x => x.StartingNeededAmountOfPoints).FirstOrDefault();
            var next = _membershipService.IQueryableGetAllAsync().Where(x => x.EndAmountOfPoints > endPoints).OrderBy(x => x.EndAmountOfPoints).FirstOrDefault();
            if (prev != null)
            {
                if (prev.Id != Guid.Empty)
                {
                    if (prev.EndAmountOfPoints > startPoints)
                    {
                        if (startPoints - 1 > prev.StartingNeededAmountOfPoints)
                        {
                            prev.EndAmountOfPoints = startPoints - 1;
                            await _membershipService.UpdateAsync(prev);
                        }
                        else
                        {
                            return $"Елементът не може да бъде променен, защото в членството ( {prev.MembershipName} )" +
                              $" крайните точки стават по-малки/равни на началните точки. Променете го!";
                        }
                    }
                }

            }
            if (next != null)
            {
                if (next.Id != Guid.Empty)
                {
                    if (next.StartingNeededAmountOfPoints < endPoints)
                    {
                        if (endPoints + 1 < next.EndAmountOfPoints)
                        {
                            next.StartingNeededAmountOfPoints = endPoints + 1;
                            await _membershipService.UpdateAsync(next);
                        }
                        else
                        {
                            return $"Елементът не може да бъде променен, защото в членството ( {next.MembershipName} )" +
                                $" началните точки стават по-големи/равни на крайните точки. Променете го!";
                        }
                    }
                }
            }

            membership.MembershipName = name;
            membership.EndAmountOfPoints = endPoints;
            membership.StartingNeededAmountOfPoints = startPoints;
            var res = await _membershipService.UpdateAsync(membership);
            return "";
        }

        public async Task DeleteMembershipHelper(Guid id, bool upper)
        {
            var membership = _membershipService.IQueryableGetAllAsync().Where(x => x.Id == id).FirstOrDefault();

            var allBooksWithinThatMembership = _bookService.IQueryableGetAllAsync().Where(x => x.NeededMembership == membership);
            if (membership != null)
            {
                if (allBooksWithinThatMembership.Count() > 0)
                {
                    var previousMembership = _membershipService.GetPreviousMembersip(id);
                    var nextMembership = _membershipService.GetNextMembersip(id);

                    if (upper)
                    {
                        if (nextMembership != null)
                        {
                            nextMembership.StartingNeededAmountOfPoints = membership!.StartingNeededAmountOfPoints;
                            await _membershipService.UpdateAsync(nextMembership);
                            foreach (var item in allBooksWithinThatMembership)
                            {
                                item.NeededMembership = nextMembership;
                            }
                        }
                        else
                        {
                            var memb = new Membership();
                            if (previousMembership != null)
                            {
                                memb.StartingNeededAmountOfPoints = previousMembership.EndAmountOfPoints + 1;
                            }
                            else
                            {
                                memb.StartingNeededAmountOfPoints = 0;
                            }
                            memb.EndAmountOfPoints = int.MaxValue;
                            memb.MembershipName = "Мастър";

                            var tempId = await _membershipService.AddAsync(memb);
                            var entity = await _membershipService.GetByIdAsync(tempId);

                            foreach (var item in allBooksWithinThatMembership)
                            {
                                item.NeededMembership = entity;
                            }
                        }

                    }
                    else
                    {

                        if (previousMembership != null)
                        {
                            previousMembership.EndAmountOfPoints = membership!.EndAmountOfPoints;
                            await _membershipService.UpdateAsync(previousMembership);
                            foreach (var item in allBooksWithinThatMembership)
                            {
                                item.NeededMembership = previousMembership;
                            }
                        }
                        else
                        {
                            var memb = new Membership();
                            memb.StartingNeededAmountOfPoints = 0;
                            if (nextMembership != null)
                            {
                                memb.EndAmountOfPoints = nextMembership.StartingNeededAmountOfPoints - 1;
                            }
                            else
                            {
                                memb.EndAmountOfPoints = int.MaxValue;
                            }
                            memb.MembershipName = "Новак";

                            var tempId = await _membershipService.AddAsync(memb);
                            var entity = await _membershipService.GetByIdAsync(tempId);

                            foreach (var item in allBooksWithinThatMembership)
                            {
                                item.NeededMembership = entity;
                            }
                        }
                    }
                }
            }

            await _bookService.UpdateRangeAsync(allBooksWithinThatMembership);

            await _membershipService.RemoveAsync(id);
        }
        #endregion
    }
}
