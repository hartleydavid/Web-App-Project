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
        public DateTime LastAirDate { get; set; }
        public string FormattedReleaseDate
        {
            get { return ReleaseDate.ToString("yyyy-MM-dd"); }
        }
        public string FormattedLastAirDate
        {
            get { return LastAirDate.ToString("yyyy-MM-dd"); }
        }
        public string ImageSrc { get; set; }
        public double IMBDScore { get; set; }
        public int Seasons { get; set; }
        public int Episodes { get; set; }


    }
}
