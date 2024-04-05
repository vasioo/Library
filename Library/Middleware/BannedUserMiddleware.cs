using Library.DataAccess.MainModels;
using Microsoft.AspNetCore.Identity;

namespace Library.Web.Middleware
{
    public class BannedUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserManager<ApplicationUser> _userManager;

        public BannedUserMiddleware(RequestDelegate next, UserManager<ApplicationUser> userManager)
        {
            _next = next;
            _userManager = userManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals("/Home/Error", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            if (context.Request.Path.Equals("/Identity", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            if (context.Request.Path.Equals("/Account/LogoutUser", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            if (context.Request.Path.Equals("/Account/AuthenticationPage", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            var user = context.User;

            if (user != null)
            {
                if (user.Identity != null)
                {
                    if (user.Identity.IsAuthenticated)
                    {
                        var appUser = await _userManager.GetUserAsync(context.User);

                        if (appUser != null && !String.IsNullOrEmpty(appUser.BanStatus.Trim()))
                        {
                            context.Response.Redirect("/Home/Error?errorCode=418"); 
                            return;
                        }
                    }
                }
            }
            await _next(context);
        }
    }
}
