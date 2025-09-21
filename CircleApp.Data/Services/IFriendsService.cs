

namespace CircleApp.Data.Services
{
    public interface IFriendsService
    {
        Task SendRequestAsync(int senderId, int receiverId);

        Task UpdateRequestAsync(int requestId, string status);

        Task RemoveFriendAsync(int friendshipId);
    }
}
