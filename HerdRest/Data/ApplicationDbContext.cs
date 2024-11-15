using Microsoft.EntityFrameworkCore;

namespace HerdRest.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string from app settings
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        }
        public DbSet<Miot> Mioty { get; set; }
        public DbSet<Krycie> Krycia { get; set; }
        public DbSet<Wydarzenie> Wydarzenia { get; set; }
        public DbSet<Locha> Lochy { get; set; }
    }
}
