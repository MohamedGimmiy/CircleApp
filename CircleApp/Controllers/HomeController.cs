using CircleApp.Controllers.Base;
using CircleApp.Data.Helpers.Enums;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CircleApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostsService _postsService;
        private readonly IHashtagsService _hashtagsService;
        private readonly IFilesService _filesService;
        public HomeController(ILogger<HomeController> logger,
            IPostsService postsService,
            IHashtagsService hashtagsService,
            IFilesService filesService)
        {
            _logger = logger;
            _postsService = postsService;
            _hashtagsService = hashtagsService;
            _filesService = filesService;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUser =GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            var allPosts = await _postsService.GetAllPostsAsync(loggedInUser.Value);
            return View(allPosts);
        }

        public async Task<IActionResult> Details(int postId)
        {
            var post = await _postsService.GetPostByIdAsync(postId);

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            var loggedInUser = GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            var imageUploadPath = await _filesService.UploadImageAsync(post.Image, ImageFileType.PostImage);
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                UserId = loggedInUser.Value,
                ImageUrl = imageUploadPath,
                NrOfReports = 0,
            };


            await _postsService.CreatePostAsync(newPost);
            await _hashtagsService.ProcessHashtagsForNewPostAsync(post.Content);
            
            //redirect use to home page
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            var loggedInUser = GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            await _postsService.TogglePostLikeAsync(postLikeVM.PostId,loggedInUser.Value);
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            var loggedInUser = GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            await _postsService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUser.Value);

           
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisiblilityVM postVisiblilityVM)
        {
            var loggedInUser = GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            await _postsService.TogglePostVisibilityAsync(postVisiblilityVM.PostId, loggedInUser.Value);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
        {
            var loggedInUser = GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            var newComment = new Comment()
            {
                PostId = postCommentVM.PostId,
                Content = postCommentVM.Content,
                UserId = loggedInUser.Value,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
            };
            await _postsService.AddPostCommentAsync(newComment);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            var loggedInUser = GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            await _postsService.ReportPostAsync(postReportVM.PostId, loggedInUser.Value);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePostComment(int commentId)
        {
            await  _postsService.RemovePostCommentAsync(commentId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostRemove(PostRemoveVM postRemoveVM)
        {
            var postRemoved = await _postsService.RemovePostAsync(postRemoveVM.PostId);

            await _hashtagsService.ProcessHashtagsForRemovePostAsync(postRemoved.Content);
            return RedirectToAction("Index");
        }
    }
}
