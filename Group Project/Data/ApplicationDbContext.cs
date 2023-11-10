using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Group_Project.Models;

namespace Group_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<Show> Shows { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Group_Project.Models.Show> Show { get; set; }
    }
}
