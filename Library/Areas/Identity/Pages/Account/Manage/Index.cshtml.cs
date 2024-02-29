// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Library.DataAccess.MainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Modum.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";


        [TempData]
        public string StatusMessage { get; set; }


        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {

            //[Phone]
            //[Display(Name = "Phone number")]
            //public string PhoneNumber { get; set; }

            public string username { get; set; } = "";
            public string lastName { get; set; } = "";
            public string firstName { get; set; } = "";
            public string email { get; set; } = "";
        }

        private async Task LoadAsync(ApplicationUser user)
        {

            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;

            Input = new InputModel
            {
                username = this.Username,
                firstName = this.FirstName,
                lastName = this.LastName,
                email = this.Email,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (Input.firstName != null)
            {
                user.FirstName = Input.firstName;
            }
            else
            {
                user.FirstName = "";
            }

            if (Input.lastName != null)
            {
                user.LastName = Input.lastName;
            }
            else
            {
                user.LastName = "";
            }
            if (Input.email != null)
            {
                user.Email = Input.email;
            }
            else
            {
                user.Email = "";
            }
            if (Input.username != null)
            {
                user.UserName = Input.username;
            }
            else
            {
                user.UserName = "";
            }
            await _userManager.UpdateAsync(user);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Вашият профил беше обновен.";
            return RedirectToPage();
        }
    }
}