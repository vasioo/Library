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
    }
}
