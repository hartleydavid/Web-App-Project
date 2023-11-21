using System;
using System.Collections.Generic;
namespace Group_Project.Models
{
    public interface Media
    {
        int Id { get; }
        string Title { get; }
        string Description { get; }
        string Genre { get; }
        DateTime ReleaseDate { get; }
        int IMBDScore { get; }
        LinkedList<Comment> Comments { get; set; }

    }
}
