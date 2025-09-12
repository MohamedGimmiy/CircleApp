
using CircleApp.Data.Models;
using Microsoft.AspNetCore.Http;

namespace CircleApp.Data.Services
{
    public interface IPostsService
    {
        Task<List<Post>> GetAllPostsAsync(int loggedInUserId);
        Task<Post> CreatePostAsync(Post post);

        Task AddPostCommentAsync(Comment comment);
        Task RemovePostCommentAsync(int commentId);
        Task<Post> RemovePostAsync(int postId);

        Task TogglePostLikeAsync(int postId, int userId);
        Task TogglePostFavoriteAsync(int postId, int userId);
        Task ReportPostAsync(int postId, int userId);
        Task TogglePostVisibilityAsync(int postId, int userId);
    }
}
