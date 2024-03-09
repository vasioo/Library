using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Services.Services
{
    public class MembershipService : BaseService<Membership>, IMembershipService
    {
        public IConfiguration Configuration { get; }
        private readonly DataContext _context;

        public MembershipService(DataContext context,IConfiguration configuration):base(configuration,context)
        {
            _context = context;
        }

        public Membership GetMembershipByName(string name)
        {
            return _context.Memberships.Where(x => x.MembershipName == name).FirstOrDefault();
        }

        public Membership GetNextMembersip(Guid id)
        {
            var currMembership = _context.Memberships.Where(x=>x.Id==id).FirstOrDefault();

            var membership = _context.Memberships.Where(x=>x.StartingNeededAmountOfPoints>currMembership.EndAmountOfPoints)
                .OrderBy(x=>x.StartingNeededAmountOfPoints)
                .FirstOrDefault();

            return membership;
        }
        public Membership GetPreviousMembersip(Guid id)
        {
            var currMembership = _context.Memberships.Where(x => x.Id == id).FirstOrDefault();

            var membership = _context.Memberships.Where(x => x.EndAmountOfPoints < currMembership.StartingNeededAmountOfPoints)
                .OrderByDescending(x => x.StartingNeededAmountOfPoints)
                .FirstOrDefault();

            return membership;
        }

        public Membership GetMembershipByPoints(int points)
        {
            return _context.Memberships.Where(x => x.StartingNeededAmountOfPoints <= points && x.EndAmountOfPoints >= points).FirstOrDefault();
        }
    }
}
