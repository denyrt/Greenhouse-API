using Greenhouse.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Greenhouse.Data.Contexts
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Hothouse> Hothouses { get; set; }

        public DbSet<LightController> LightControllers { get; set; }
        public DbSet<TemperatureController> TemperatureControllers { get; set; }
        public DbSet<WetController> WetControllers { get; set; }

        public DbSet<TemperatureReport> TemperatureReports { get; set; }
        public DbSet<LightReport> LightReports { get; set; }
        public DbSet<WetReport> WetReports { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        
        }
    }
}