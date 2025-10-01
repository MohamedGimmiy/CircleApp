using CircleApp.Controllers.Base;
using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Helpers.Enums;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CircleApp.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.User}")]

    public class SettingsController : BaseController
    {
        private readonly IUsersService _usersService;
        private readonly IFilesService _filesService;
        private readonly UserManager<User> _userManager;
        public SettingsController(IUsersService usersService, 
            IFilesService filesService, 
            UserManager<User> userManager)
        {
            _usersService = usersService;
            _filesService = filesService;
            _userManager = userManager;
        }

        public async Task< IActionResult> Index()
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            return View(loggedInUser);
        }

        [HttpPost]
        public async Task <IActionResult> UpdateProfilePicture(UpdateProfilePictureVM profilePictureVM)
        {
            var loggedInUser = GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            var uploadedProfilePictureUrl = await _filesService
                .UploadImageAsync(profilePictureVM.ProfilePictureImage, ImageFileType.ProfilePicture);

            await _usersService.UpdateUserProfilePicture(loggedInUser.Value, uploadedProfilePictureUrl);

            return RedirectToAction("Index");
        }
    }
}
