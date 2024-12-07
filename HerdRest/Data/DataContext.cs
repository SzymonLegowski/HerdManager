using HerdRest.Model;
using Microsoft.EntityFrameworkCore;

namespace HerdRest.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Miot> Mioty { get; set; }
        public DbSet<Wydarzenie> Wydarzenia { get; set; }
        public DbSet<Locha> Lochy { get; set; }

       
    }
}
