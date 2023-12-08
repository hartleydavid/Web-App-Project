using System;

namespace Group_Project.Models
{
    public class Like
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public DateTime LikedAt { get; set; }

    }

}
