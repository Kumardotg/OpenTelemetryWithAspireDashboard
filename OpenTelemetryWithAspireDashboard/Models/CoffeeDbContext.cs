using Microsoft.EntityFrameworkCore;

namespace OpenTelemetryWithAspireDashboard
{
    public class CoffeeDbContext : DbContext
    {
        public CoffeeDbContext(DbContextOptions<CoffeeDbContext> options) : base(options)
        {

        }

        public DbSet<Sale> Sales { get; set; }

    }
}