using HerdRest.Model;
using Microsoft.EntityFrameworkCore;

namespace HerdRest.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        public DbSet<Miot> Mioty { get; set; }
        public DbSet<Wydarzenie> Wydarzenia { get; set; }
        public DbSet<Locha> Lochy { get; set; }
        public DbSet<WydarzenieLocha> WydarzeniaLochy { get; set; }
        public DbSet<WydarzenieMiot> WydarzeniaMiotu { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Locha>()
                .Property(l => l.DataCzasModyfikacji)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'Europe/Warsaw'")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Locha>()
                .Property(l => l.DataCzasUtworzenia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'Europe/Warsaw'")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Wydarzenie>()
                .Property(w => w.DataCzasModyfikacji)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'Europe/Warsaw'")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Wydarzenie>()
                .Property(w => w.DataCzasUtworzenia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'Europe/Warsaw'")
                .ValueGeneratedOnAdd();
                
            modelBuilder.Entity<Miot>()
                .Property(m => m.DataCzasModyfikacji)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'Europe/Warsaw'")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Miot>()
                .Property(m => m.DataCzasUtworzenia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'Europe/Warsaw'")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<WydarzenieLocha>()
                .HasKey(wl => new { wl.WydarzenieId, wl.LochaId});
            modelBuilder.Entity<WydarzenieLocha>()
                .HasOne(w => w.Wydarzenie)
                .WithMany(wl => wl.WydarzeniaLoch)
                .HasForeignKey(w => w.WydarzenieId);
            modelBuilder.Entity<WydarzenieLocha>()
                .HasOne(w => w.Locha)
                .WithMany(wl => wl.WydarzeniaLochy)
                .HasForeignKey(l => l.LochaId);

            modelBuilder.Entity<WydarzenieMiot>()
                .HasKey(wm => new { wm.WydarzenieId, wm.MiotId});
            modelBuilder.Entity<WydarzenieMiot>()
                .HasOne(w => w.Wydarzenie)
                .WithMany(wm => wm.WydarzeniaMioty)
                .HasForeignKey(w => w.WydarzenieId);
            modelBuilder.Entity<WydarzenieMiot>()
                .HasOne(m => m.Miot)
                .WithMany(wm => wm.WydarzeniaMiotu)
                .HasForeignKey(m => m.MiotId);
        }
    }


}

