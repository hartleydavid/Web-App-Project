using System;
using System.Collections.Generic;
namespace Group_Project.Models
{
    public class Comment
    {

        public int Id { get; set; }

        //Foreign Key to connect comment to show/movie?
        public int MediaID { get; set; }
        public string Text { get; set; }

    }
}
