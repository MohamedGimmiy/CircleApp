using CircleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Services
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext _context;

        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUser(int loggedInUserId)
        {
            return await _context.Users.FirstOrDefaultAsync(n => n.Id == loggedInUserId) ?? new User();
        }

        public async Task<List<Post>> GetUserPosts(int userId)
        {
            var allPosts = await _context
                            .Posts
                            .Where(n =>  n.UserId == userId
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

        public async Task UpdateUserProfilePicture(int userId, string profilePictureUrl)
        {
            var userDb = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (userDb != null)
            {
                userDb.ProfilePictureUrl = profilePictureUrl;
                _context.Users.Update(userDb);
                await _context.SaveChangesAsync();
            }
        }


    }
}
