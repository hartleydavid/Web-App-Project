using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Group_Project.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int MediaID { get; set; }
        public string Text { get; set; }
        [ForeignKey("Author")]
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
