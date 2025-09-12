

namespace CircleApp.Data.Models
{
    public class HashTag
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
