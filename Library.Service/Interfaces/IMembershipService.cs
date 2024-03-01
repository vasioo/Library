using Library.Models.BaseModels;

namespace Library.Services.Interfaces
{
    public interface IMembershipService : IBaseService<Membership>
    {
        Membership GetMembershipByName(string name);
    }
}
