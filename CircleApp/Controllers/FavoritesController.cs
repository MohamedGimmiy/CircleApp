using CircleApp.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CircleApp.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly IPostsService _postsService;

        public FavoritesController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        public async Task<IActionResult> Index()
        {
            int loggedInUser = 1;
            var myFavoritePosts = await _postsService.GetAllFavoritedPostsAsync(loggedInUser);
            return View(myFavoritePosts);
        }
    }
}
