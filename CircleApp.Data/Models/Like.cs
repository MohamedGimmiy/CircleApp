

using System.ComponentModel.DataAnnotations;

namespace CircleApp.Data.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }

        public Post Post { get; set; }
        public User User { get; set; }

    }
}
