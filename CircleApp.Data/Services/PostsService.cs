using CircleApp.Data.Dtos;
using CircleApp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace CircleApp.Data.Services
{
    public class PostsService : IPostsService
    {
        private readonly AppDbContext _context;
        private readonly INotificationsService _notificationsService;

        public PostsService(AppDbContext context, INotificationsService notificationsService)
        {
            _context = context;
            _notificationsService = notificationsService;
        }

        public async Task<List<Post>> GetAllPostsAsync(int loggedInUser)
        {
            var allPosts = await _context
                            .Posts
                            .Where(n => (n.IsPrivate == false || n.UserId == loggedInUser)
                            && n.Reports.Count < 5 && !n.IsDeleted)
                            .Include(n => n.User)
                            .Include(n => n.Likes)
                            .Include(n => n.Comments).ThenInclude(n => n.User)
                            .Include(n => n.Favorites)
                            .Include(n => n.Reports)
                            .OrderByDescending(p => p.DateCreated)
                            .ToListAsync();

            return allPosts;
        }
        public async Task<Post> GetPostByIdAsync(int postId)
        {
            var postDb = await _context
                            .Posts
                            .Include(n => n.User)
                            .Include(n => n.Likes)
                            .Include(n => n.Comments).ThenInclude(n => n.User)
                            .Include(n => n.Favorites)
                            .FirstOrDefaultAsync(n => n.Id == postId);

            return postDb;
        }
        public async Task<List<Post>> GetAllFavoritedPostsAsync(int loggedInUserId)
        {


            var favoritedPosts = await _context.Favorites
                .Include(n => n.Post.Reports)
                    .Include(n => n.Post.User)
                    .Include(p => p.Post.Comments)
                        .ThenInclude(c => c.User)
                    .Include(n => n.Post.Likes)
                    .Include(n => n.Post.Favorites)
                .Where(n => n.UserId == loggedInUserId && 
                !n.Post.IsDeleted && 
                n.Post.Reports.Count < 5)
                .OrderByDescending(n=> n.DateCreated)
                .Select(n => n.Post)

                .ToListAsync();



            return favoritedPosts;
        }
        public async Task<Post> CreatePostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> RemovePostAsync(int postId)
        {
            var postDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if(postDb != null)
            {
                //_context.Posts.Remove(postDb);
                postDb.IsDeleted = true;
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();
            }
            return postDb;
        }
        public async Task RemovePostCommentAsync(int commentId)
        {
            var commentDb = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if(commentDb != null)
            {
                _context.Comments.Remove(commentDb);    
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddPostCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task ReportPostAsync(int postId, int userId)
        {
            var newReport = new Report()
            {
                PostId = postId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };

            await _context.Reports.AddAsync(newReport);
            await _context.SaveChangesAsync();

            var post = await _context.Posts.FirstOrDefaultAsync(n => n.Id == postId);

            if (post != null)
            {
                post.NrOfReports++;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<GetNotificationDto> TogglePostFavoriteAsync(int postId, int userId)
        {

            var response = new GetNotificationDto()
            {
                Success = true,
                SendNotification = false
            };
            var favorite = await _context.Favorites
                           .Where(l => l.PostId == postId && l.UserId == userId)
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
                    PostId = postId,
                    UserId = userId,
                    DateCreated = DateTime.UtcNow
                };
                await _context.Favorites.AddAsync(newFavorite);
                await _context.SaveChangesAsync();
                response.SendNotification = true;
            }

            return response;
        }

        public async Task<GetNotificationDto> TogglePostLikeAsync(int postId, int userId)
        {

            var response = new GetNotificationDto()
            {
                Success = true,
                SendNotification = false
            };

            var like = await _context.Likes
                            .Where(l => l.PostId == postId && l.UserId == userId)
                            .FirstOrDefaultAsync();

            if ((like != null))
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postId,
                    UserId = userId,
                };
                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();

                response.SendNotification = true;

            }
            return response;
        }

        public async Task TogglePostVisibilityAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(l => l.Id == postId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if ((post != null))
            {
                post.IsPrivate = !post.IsPrivate;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
        }


    }
}
