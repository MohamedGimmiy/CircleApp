


using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Data.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly AppDbContext _context;

        public FriendsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateRequestAsync(int requestId, string newStatus)
        {
            var requestDb = await _context.FriendRequests.FirstOrDefaultAsync(f => f.Id == requestId);
            if (requestDb != null)
            {
                requestDb.Status = newStatus;
                requestDb.DateUpdated = DateTime.UtcNow;
                _context.FriendRequests.Update(requestDb);

                await _context.SaveChangesAsync();
                if(newStatus == FriendshipStatus.Accepted)
                {
                    var friendship = new Friendship()
                    {
                        SenderId = requestDb.SenderId,
                        ReceiverId = requestDb.ReceiverId,
                        DateCreated = DateTime.UtcNow,
                    };

                    await _context.Friendships.AddAsync(friendship);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task SendRequestAsync(int senderId, int receiverId)
        {
            var request = new FriendRequest()
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                Status = FriendshipStatus.Pending
            };
            await _context.FriendRequests.AddAsync(request);

            await _context.SaveChangesAsync();

        }

        public async Task RemoveFriendAsync(int friendshipId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(x => x.Id == friendshipId);

            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();

            }
        }

    }
}
