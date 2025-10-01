using CircleApp.Controllers.Base;
using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Helpers.Enums;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


namespace CircleApp.Controllers
{
    [Authorize(Roles = AppRoles.User)]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostsService _postsService;
        private readonly IHashtagsService _hashtagsService;
        private readonly IFilesService _filesService;
        private readonly INotificationsService _notificationsService;
        public HomeController(ILogger<HomeController> logger,
            IPostsService postsService,
            IHashtagsService hashtagsService,
            IFilesService filesService,
            INotificationsService notificationsService)
        {
            _logger = logger;
            _postsService = postsService;
            _hashtagsService = hashtagsService;
            _filesService = filesService;
            _notificationsService = notificationsService;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (userId == null)
            {
                return RedirectToLogin();
            }

            var result = await _postsService.TogglePostLikeAsync(postLikeVM.PostId,userId.Value);
            var post = await _postsService.GetPostByIdAsync(postLikeVM.PostId);

            if (result.SendNotification && userId != post.UserId)
            {
                await _notificationsService
                    .AddNotificationAsync(post.UserId,
                    NotificationType.Like,
                    userName,
                    postLikeVM.PostId);

            }
            return PartialView("Home/_Post", post);
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (userId == null)
            {
                return RedirectToLogin();
            }
            var result = await _postsService.TogglePostFavoriteAsync(postFavoriteVM.PostId, userId.Value);

            var post = await _postsService.GetPostByIdAsync(postFavoriteVM.PostId);


            if (result.SendNotification && userId != post.UserId)
            {
                await _notificationsService
                    .AddNotificationAsync(post.UserId,
                    NotificationType.Favorite,
                    userName,
                    postFavoriteVM.PostId);

            }
            return PartialView("Home/_Post", post);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (userId == null)
            {
                return RedirectToLogin();
            }
            var newComment = new Comment()
            {
                PostId = postCommentVM.PostId,
                Content = postCommentVM.Content,
                UserId = userId.Value,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
            };
             await _postsService.AddPostCommentAsync(newComment);

            var post = await _postsService.GetPostByIdAsync(postCommentVM.PostId);

            if( userId != post.UserId)
            {
                await _notificationsService
                    .AddNotificationAsync(post.UserId,
                    NotificationType.Comment,
                    userName,
                    postCommentVM.PostId);
            }
            
            return PartialView("Home/_Post", post);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePostComment(int commentId, int postId)
        {
            await  _postsService.RemovePostCommentAsync(commentId);

            var post =await _postsService.GetPostByIdAsync(postId);
            return PartialView("Home/_Post", post);
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
