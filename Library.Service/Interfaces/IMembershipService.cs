﻿using Library.DataAccess.MainModels;
using Library.Models.BaseModels;

namespace Library.Services.Interfaces
{
    public interface IMembershipService : IBaseService<Membership>
    {
        Membership GetMembershipByName(string name);
        Membership GetPreviousMembersip(Guid id);
        Membership GetNextMembersip(Guid id);
        Membership GetMembershipByPoints(int points);
        Task<int> GetHoursWhichCanBeReadByMembership(ApplicationUser user);
    }
}
