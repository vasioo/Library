// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Library.DataAccess.MainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Library.Services.Interfaces;
using Library.Models.Cloudinary;
using Library.Services.Services;

namespace Modum.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IBookService _baseService;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IBookService baseService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _baseService = baseService;
        }

        public string Id { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Image { get; set; } = "";


        [TempData]
        public string StatusMessage { get; set; }


        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {

            public string username { get; set; } = "";
            public string lastName { get; set; } = "";
            public string firstName { get; set; } = "";
            public string email { get; set; } = "";
        }

        private async Task LoadAsync(ApplicationUser user)
        {

            Id = user.Id;
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
                return NotFound($"Не може да бъде намерен потребител с ИН: '{_userManager.GetUserId(User)}'.");
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

            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                if (file != null && file.Length > 0)
                {
                    var photo = new Photo();

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        byte[] bytes = memoryStream.ToArray(); 
                        string base64String = Convert.ToBase64String(bytes); 
                        photo.Image = $"data:{file.ContentType};base64,{base64String}";
                    }
                    if (file.FileName != null && file.FileName != "")
                    {
                        photo.ImageName = $"profile-image-for-{user.Id}";
                        photo.PublicId = $"profile-image-for-{user.Id}";
                    }
                    await _baseService.DeleteImage($"https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/{photo.PublicId}");
                    await _baseService.SaveImage(photo);
                }
            }

            await _userManager.UpdateAsync(user);
            if (user == null)
            {
                return NotFound($"Няма потребител с ID: '{_userManager.GetUserId(User)}'.");
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