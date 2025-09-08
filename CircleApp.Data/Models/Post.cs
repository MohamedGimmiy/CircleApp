using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CircleApp.Data.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPrivate {  get; set; }
        public int NrOfReports { get; set; }
        public DateTime DateCreated {  get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsDeleted { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        //Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
