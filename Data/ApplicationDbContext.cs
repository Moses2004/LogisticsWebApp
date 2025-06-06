using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LogisticsWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace LogisticsWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Orderline> Orderlines { get; set; }
        public DbSet<Lorry> Lorries { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Assistant> Assistants { get; set; }

    }
}
