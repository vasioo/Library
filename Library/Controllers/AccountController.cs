using Library.DataAccess.MainModels;
using Library.Web.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modum.Web.Areas.Identity.Pages.Account;
using System.ComponentModel.DataAnnotations;

namespace Library.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _signInLogger;
        private readonly ILogger<RegisterModel> _signUpLogger;
        private readonly UserManager<ApplicationUser> _userManager;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public AccountController(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> signInLogger, ILogger<RegisterModel> signUpLogger, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _signInLogger = signInLogger;
            _signUpLogger = signUpLogger;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SignIn(string email, string password, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _signInLogger.LogInformation("User logged in.");
                    return Json(new { success = true, redirectUrl = returnUrl });
                }
                else
                {
                    var errorMessage = "Invalid login attempt.";
                    TempData["LoginErrorMessage"] = errorMessage;

                    return Json(new { success = false, message = errorMessage });
                }
            }

            var errMsg = "An Err Conflicted.";

            return Json(new { success = false, message = errMsg });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SignUp(string email, string password, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            try
            {
                var user = new ApplicationUser();

                user.UserName = email;
                user.Email = email;

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Json(new { success = true, redirectUrl = returnUrl });
                    #region Send Email

                    // _logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //  await _signInManager.SignInAsync(user, isPersistent: false);
                    //  return LocalRedirect(returnUrl);
                    //}
                    #endregion
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                var errorMessage = "Invalid login attempt.";
                TempData["LoginErrorMessage"] = errorMessage;

                return Json(new { success = false, message = errorMessage });
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
