using CircleApp.Controllers.Base;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CircleApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService _userService;
        private readonly UserManager<User> _userManager;
        public UsersController(IUsersService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Details(int userId)
        {
            var userPosts = await _userService.GetUserPosts(userId);
            var user = await _userService.GetUser(userId);
            //var user = await _userManager.FindByIdAsync(userId.ToString());

            var userprofileVm = new GetUserProfileVM()
            {
                Posts = userPosts,
                User = user
            };
            return View(userprofileVm);
        }
    }
}
