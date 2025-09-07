using System.Diagnostics;
using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context )
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int loggedInUser = 1;
            var allPosts = await  _context
                .Posts
                .Where(n => n.IsPrivate == false || n.UserId == loggedInUser)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Comments).ThenInclude(n => n.User)
                .Include(n=> n.Favorites)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();
            return View(allPosts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            int loggedInUser = 1;
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                UserId = loggedInUser,
                ImageUrl = "",
                NrOfReports = 0,
            };

            if(post.Image != null && post.Image.Length >0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (post.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/Uploaded");
                    Directory.CreateDirectory(rootFolderPathImages);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);

                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using(var stream = new FileStream(filePath, FileMode.Create))
                        await post.Image.CopyToAsync(stream);
                    newPost.ImageUrl = "/images/Uploaded/" + fileName;


                }

            }
            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();

            //redirect use to home page
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            int loggedInUser = 1;

            var like = await _context.Likes
                .Where(l => l.PostId == postLikeVM.PostId && l.UserId == loggedInUser)
                .FirstOrDefaultAsync();

            if ((like  != null))
            {
                 _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            } else
            {
                var newLike = new Like()
                {
                    PostId = postLikeVM.PostId,
                    UserId = loggedInUser,
                };
                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            int loggedInUser = 1;

            var favorite = await _context.Favorites
                .Where(l => l.PostId == postFavoriteVM.PostId && l.UserId == loggedInUser)
                .FirstOrDefaultAsync();

            if ((favorite != null))
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postFavoriteVM.PostId,
                    UserId = loggedInUser,
                };
                await _context.Favorites.AddAsync(newFavorite);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisiblilityVM postVisiblilityVM)
        {
            int loggedInUser = 1;

            var post = await _context.Posts
                .Where(l => l.Id == postVisiblilityVM.PostId && l.UserId == loggedInUser)
                .FirstOrDefaultAsync();

            if ((post != null))
            {
                post.IsPrivate = !post.IsPrivate;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
        {
            int loggedInUser = 1;

            var newComment = new Comment()
            {
                PostId = postCommentVM.PostId,
                Content = postCommentVM.Content,
                UserId = loggedInUser,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
            };
            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePostComment(int commentId)
        {
            var commentDb = await _context.Comments
                .Where(n => n.Id == commentId).FirstOrDefaultAsync();


            if(commentDb != null)
            {
                _context.Comments.Remove(commentDb);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }



    }
}
