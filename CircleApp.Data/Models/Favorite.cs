

namespace CircleApp.Data.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }

        public Post Post { get; set; }
        public User User { get; set; }
    }
}
