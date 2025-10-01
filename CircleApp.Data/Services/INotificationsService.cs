using CircleApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Services
{
    public interface INotificationsService
    {
        Task AddNotificationAsync(int userId, string notificationType, string userFullname, int? postId);

        Task<int> GetUnreadNotificationsAsync(int userId);

        Task<List<Notification>> GetNotificationsAsync(int userId);

        Task SetNotificationAsReadAsync(int notificationId);
    }
}
