using System;
using System.Collections.Generic;

namespace Group_Project.Models
{
    public class Movie : Media
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int BoxOfficeTotal { get; set; }
        public int IMBDScore { get; set; }
        public LinkedList<Comment> Comments { get; set; }

    }
}
