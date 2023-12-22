using Library.DataAccess.MainModels;
using Library.Web.Controllers.AdminControllerHelper;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;

namespace Library.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddScoped<UserManager<ApplicationUser>>();

            service.AddScoped<IHomeControllerHelper, HomeControllerHelper>();
            service.AddScoped<IAdminControllerHelper, AdminControllerHelper>();
        }
    }
}
