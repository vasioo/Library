using Library.DataAccess.MainModels;
using Library.DataAccess;
using Microsoft.AspNetCore.Identity;
using Stripe.Climate;

namespace Library.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection service)
        {
           
            service.AddScoped<UserManager<ApplicationUser>>();
        }
    }
}
