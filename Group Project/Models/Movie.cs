using System;
using System.Collections.Generic;

namespace Group_Project.Models
{
    public class Movie : IMedia
    {

        /**
         * 50ec474a2af5a633e519c55f84f1a010
         * 
         * eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MGVjNDc0YTJhZjVhNjMzZTUxOWM1NWY4NGYxYTAxMCIsInN1YiI6IjY1NWQ0OWZmZmFiM2ZhMDBmZWNjZjk4NiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.EvJfo5-I1AS2ro4I8mWfrzSHKUEuHQJQR_KolK-WSHs
         */


        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string FormattedReleaseDate
        {
            get { return ReleaseDate.ToString("yyyy-MM-dd"); }
        }
        public string ImageSrc { get; set; }
        public int BoxOfficeTotal { get; set; }
        public double IMBDScore { get; set; }

    }
}
