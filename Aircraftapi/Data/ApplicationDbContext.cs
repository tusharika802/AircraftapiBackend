using Aircraftapi.Models;
using Microsoft.EntityFrameworkCore;

namespace Aircraftapi.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            :base(options)
        {
            
        }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<ServiceCentre> ServiceCentres { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Staff> Staffs { get; set; }
    }
}
