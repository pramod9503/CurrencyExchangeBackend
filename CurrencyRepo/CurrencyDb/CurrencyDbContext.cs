using Microsoft.EntityFrameworkCore;
using CurrencyRepo.Models.BackModels;

namespace CurrencyRepo.CurrencyDb
{
    public class CurrencyDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }

        public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Currency>((entity) => 
            {
                entity.ToTable("Currencies");
                entity.HasKey(x => x.Id).HasName("PK_CurrencyId");
                entity.Property(x => x.Id).ValueGeneratedOnAdd();
                entity.HasIndex(x => x.Country).IsUnique();
                entity.Property(x => x.Rate).HasColumnType("DECIMAL(10,6)").HasPrecision(10, 6);
            });
        }
    }
}
