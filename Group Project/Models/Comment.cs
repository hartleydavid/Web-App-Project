using System.Collections.Generic;
namespace Group_Project.Models
{
    public class Comment
    {

        public int Id { get; set; }

        //Foreign Key to connect comment to show/movie?
        public int MediaID { get; set; }
        //If a given comment has a reply
        //public LinkedList<Comment> Replies { get; set; }
    }
}
