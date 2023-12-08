using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Group_Project.Models;
using System.Reflection.Emit;

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Like>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Movie)
                .WithMany(m => m.Likes)
                .HasForeignKey(l => l.MovieId);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId);
        }

        public DbSet<Group_Project.Models.Show> Show { get; set; }
        public DbSet<Group_Project.Models.Movie> Movie { get; set; }
        public DbSet<Group_Project.Models.Comment> Comment { get; set; }
        public DbSet<Group_Project.Models.Like> Likes { get; set; }

    }
}
