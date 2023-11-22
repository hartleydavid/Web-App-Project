using System;
using System.Collections.Generic;

namespace Group_Project.Models
{
    public class Show : IMedia
    {
        //Image object?

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        //SQL keys instead of an object?
        //public LinkedList<Comment> Comments { get; set; }
        public int IMBDScore { get; set; }
        public int Seasons { get; set; }
        public int Episodes { get; set; }


    }
}
