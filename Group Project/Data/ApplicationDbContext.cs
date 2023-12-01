using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Group_Project.Models;

namespace Group_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        // public DbSet<Show> Shows { get; set; }
        // public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Group_Project.Models.Show> Show { get; set; }
        public DbSet<Group_Project.Models.User> User { get; set; }
        public DbSet<Group_Project.Models.Movie> Movie { get; set; }
        public DbSet<Group_Project.Models.Comment> Comment { get; set; }
    }
}
