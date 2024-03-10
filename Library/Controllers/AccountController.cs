using Library.DataAccess.MainModels;
using Library.Models.UserModels;
using Library.Models.UserModels.Interfaces;
using Library.Web.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Modum.Web.Areas.Identity.Pages.Account;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text;
using Library.Services.Interfaces;

namespace Library.Web.Controllers
{
    public class AccountController : Controller
    {
        #region FieldsAndConstructor
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _signInLogger;
        private readonly ILogger<RegisterModel> _signUpLogger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSenderService _emailSenderService;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public AccountController(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> signInLogger, ILogger<RegisterModel> signUpLogger,
            UserManager<ApplicationUser> userManager, IEmailSenderService emailSenderService)
        {
            _signInManager = signInManager;
            _signInLogger = signInLogger;
            _signUpLogger = signUpLogger;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
        }

        #endregion

        #region AuthPage
        public IActionResult AuthenticationPage()
        {
            return View("~/Views/Account/AuthenticationPage.cshtml");
        }
        #endregion

        #region SignIn

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

        #endregion

        #region SignUp

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

        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(IdentityModel model)
        {
            var user =await _userManager.FindByEmailAsync(model.LoginEmail);
            if (user==null)
            {
                return Json(new { success = false, message = "Невалиден опит. Грешка в данните." });
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.LoginPassword, true,false);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Потребителя влезе в приложението." });
            }
            if (result.RequiresTwoFactor)
            {
                return Json(new { success = false, message = "Нужна двуфактурна автентикация.", redirectPage = "./LoginWith2fa", rememberMe = true });
            }
            if (result.IsLockedOut)
            {
                return Json(new { success = false, message = "Аккаунта е заключен." });
            }
            else
            {
                return Json(new { success = false, message = "Невалиден опит. Грешка в данните." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Register(IdentityModel model)
        {
            try
            {
                var existingEmailUser = await _userManager.FindByEmailAsync(model.RegisterEmail);
                if (existingEmailUser != null)
                {
                    return Json(new { success = false, message = "Email already exists." });
                }
                var existingUsernameUser = await _userManager.FindByNameAsync(model.RegisterUsername);
                if (existingUsernameUser != null)
                {
                    return Json(new { success = false, message = "Username already exists.", usernameRelated = true });
                }
                var user = new ApplicationUser();

                user.UserName = model.RegisterUsername;
                user.Email = model.RegisterEmail;
                var result = await _userManager.CreateAsync(user, model.RegisterPassword);


                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    string returnUrl = "";
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    var emailBody = $@"
                         <html>
                           <body>
                               <div style='text-align: center;'>
                                   <img src='https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/litify-logo_fwtfvq' alt='Website Logo' style='height: 20em;' />
                               </div>
                               <br/>
                               <hr/>
                               <br/>
                               <div style='text-align: center; font-size: 20px;'>
                                   Потвърдете акаунта си като <br/>
                                   <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>натиснете тук</a>.
                               </div>
                            </body>
                        </html>";

                    _emailSenderService.SendEmail(model.RegisterEmail, emailBody, "Потвърдете акаунта си");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return Json(new { success = true, message = "Проверете имейла си за потвърждение!" });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Json(new { success = true, message = "Успешно създаване на акаунт." });
                    }

                }
                return Json(new { success = false, message = "Невалидно създаване на акаунт." });
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
