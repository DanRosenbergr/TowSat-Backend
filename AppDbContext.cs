using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TowSat_Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TowSat_Backend {
    public class AppDbContext : IdentityDbContext<AppUser> {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }       

        public DbSet<BtsData> BtsData { get; set; }

        public DbSet<GpsData> GpsData { get; set; }


    }
}
