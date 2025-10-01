

using System.ComponentModel.DataAnnotations.Schema;

namespace CircleApp.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string Message { get; set; }
        public bool IsRead {  get; set; }
        public string Type { get; set; }
        public int? PostId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
