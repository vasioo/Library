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

            service.AddScoped<IBaseService<Notification>, BaseService<Notification>>();
            service.AddScoped<INotificationService, NotificationService>();

            service.AddScoped<IBaseService<Book>, BaseService<Book>>();
            service.AddScoped<IBookService, BookService>();

            service.AddScoped<IBaseService<BookCategory>, BaseService<BookCategory>>();
            service.AddScoped<IBookCategoryService, BookCategoryService>();

            service.AddScoped<IBaseService<BookSubject>, BaseService<BookSubject>>();
            service.AddScoped<IBookSubjectService, BookSubjectService>();

            service.AddScoped<UserManager<ApplicationUser>>();

        }
    }
}
