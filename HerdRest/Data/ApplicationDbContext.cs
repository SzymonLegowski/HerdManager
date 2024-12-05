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
        public DbSet<Miot>? Mioty { get; set; }
        public DbSet<Wydarzenie>? Wydarzenia { get; set; }
        public DbSet<Locha>? Lochy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wydarzenie>()
            .HasMany(w => w.Mioty)
            .WithMany(m => m.Wydarzenia)
            .UsingEntity(j => j.ToTable("WydarzenieMiot"));

            modelBuilder.Entity<Locha>()
            .HasMany(l => l.Wydarzenia)
            .WithMany(w => w.Lochy)
            .UsingEntity(j => j.ToTable("WydarzenieLocha"));

            modelBuilder.Entity<Miot>()
            .HasOne(m => m.Locha)
            .WithMany(l => l.Mioty)
            .HasForeignKey(m => m.LochaId);
        }
    }
}
