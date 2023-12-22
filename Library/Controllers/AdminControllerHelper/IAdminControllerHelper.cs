using Library.Models.ViewModels;

namespace Library.Web.Controllers.AdminControllerHelper
{
    public interface IAdminControllerHelper
    {
        Task<StatisticsViewModel> StatisticsHelper();
    }
}
