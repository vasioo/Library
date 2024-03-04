using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Models.ViewModels;

namespace Library.Web.Controllers.AdminControllerHelper
{
    public interface IAdminControllerHelper
    {
        Task<StatisticsViewModel> StatisticsHelper();
        IQueryable<Membership> GetMemberships();
        Task<string> AddMembershipHelper(string name,int startPoints, int points);
        Task<string> EditMembershipHelper(Guid id, string name, int startPoints, int endPoints);
        Task DeleteMembershipHelper(Guid id,bool upper);
        Task<IEnumerable<ReportBookDTO>> GetBookInformationByTimeAndCount(DateTime startDate, DateTime endDate, int selectedCountOfItems);
        Task<List<string>> GetGenreInformationByTimeAndCount(DateTime startDate, DateTime endDate);
    }
}
