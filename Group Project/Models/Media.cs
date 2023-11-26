using System;
using System.Collections.Generic;
namespace Group_Project.Models
{
    public interface IMedia
    {
        //Image object?
        int Id { get; }
        string Title { get; }
        string Description { get; }
        string Genre { get; }
        DateTime ReleaseDate { get; }
        double IMBDScore { get; }

        //SQL keys instead of an object?
        //LinkedList<Comment> Comments { get; set; }

    }
}
