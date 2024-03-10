﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Library.DataAccess.MainModels;
using Library.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace Modum.Web.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSenderService _emailSenderService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _emailSenderService = emailSenderService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage ="Имейлът е задължителен")]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code, Input.Email },
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
                               <div style='text-align: center; font-size: 30px;'>
                                   Сменете паролата си като <br/>
                                   <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>натиснете тук</a>.
                               </div>
                            </body>
                        </html>";

                _emailSenderService.SendEmail(Input.Email, emailBody, "Сменете паролата си");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
