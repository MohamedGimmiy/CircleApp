using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CircleApp.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected int? GetUserId()
        {
            var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUser))
            {
                return null;
            }
            return int.Parse(loggedInUser);
        }

        protected string GetUserFullName()
        {
            var loggedInUserFullName = User.FindFirstValue(ClaimTypes.Name);

            return loggedInUserFullName;
        }
        protected IActionResult RedirectToLogin() {
            return RedirectToAction("Login", "Authentication");
        }
    }
}
