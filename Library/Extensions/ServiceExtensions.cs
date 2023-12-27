using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using Library.Services.Services;
using Library.Web.Controllers.AdminControllerHelper;
using Library.Web.Controllers.HomeControllerHelper;
using Microsoft.AspNetCore.Identity;

namespace Library.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddScoped<DataContext, DataContext>();

            service.AddScoped<IHomeControllerHelper, HomeControllerHelper>();
            service.AddScoped<IAdminControllerHelper, AdminControllerHelper>();
            service.AddScoped<ILibrarianControllerHelper, LibrarianControllerHelper>();

            service.AddScoped<IBaseService<UserLeasedBookMappingTable>, BaseService<UserLeasedBookMappingTable>>();
            service.AddScoped<IUserLeasedBookService, UserLeasedBookService>();

            service.AddScoped<UserManager<ApplicationUser>>();

        }
    }
}
