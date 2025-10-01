

using CircleApp.Data.Dtos;
using CircleApp.Data.Models;

namespace CircleApp.Data.Services
{
    public interface IFriendsService
    {
        Task SendRequestAsync(int senderId, int receiverId);

        Task<FriendRequest> UpdateRequestAsync(int requestId, string status);

        Task RemoveFriendAsync(int friendshipId);
        Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId);
        Task<List<FriendRequest>> GetSentFriendRequestAsync(int userId);
        Task<List<FriendRequest>> GetReceivedFriendRequestAsync(int userId);

        Task<List<Friendship>> GetFriendsAsync(int userId);
    }
}
