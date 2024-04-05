using Library.DataAccess.MainModels;
using Library.Models.UserModels;
using Library.Services.Interfaces;
using Library.Web.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.WebUtilities;
using Modum.Web.Areas.Identity.Pages.Account;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

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


        public async Task<IActionResult> LogoutUser()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("AuthenticationPage", "Account");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(IdentityModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.LoginEmail);
            if (user == null)
            {
                return Json(new { success = false, message = "Невалиден опит. Грешка в данните." });
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.LoginPassword, true, false);
            if (result.Succeeded)
            {
                if (!String.IsNullOrEmpty(user.BanStatus.Trim()))
                {
                    return Json(new { success = false, message = "Вашият профил е блокиран от приложението!" });
                }
                return Json(new { success = true, message = "" });
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
                    return Json(new { success = false, message = "Имейлът има вече аккаунт." });
                }
                var existingUsernameUser = await _userManager.FindByNameAsync(model.RegisterUsername);
                if (existingUsernameUser != null)
                {
                    return Json(new { success = false, message = "Потребителското име вече съществува.", usernameRelated = true });
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
                                   <div style='text-align: center; font-size: 15px; font-weight: bold; margin-top: 20px; text-decoration:none; color: #d4af37;'>
                                      Вие ще получите 5 точки бонус при потвърждение на акаунта
                                   </div>
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
                        user.Points += 5;
                        await _userManager.UpdateAsync(user);
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



        #endregion

    }
}
