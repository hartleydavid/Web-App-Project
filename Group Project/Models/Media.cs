using System;
using System.Collections.Generic;
namespace Group_Project.Models
{
    public interface IMedia
    {
        int Id { get; }
        string Title { get; }
        string Description { get; }
        string Genre { get; }
        DateTime ReleaseDate { get; }
        public string FormattedReleaseDate { get; }
        double IMBDScore { get; }
        public string ImageSrc { get; set; }

        //SQL keys instead of an object?
        public List<Comment> Comments { get; set; }
    }
}
