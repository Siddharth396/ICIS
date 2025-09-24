namespace Infrastructure.SQLDB
{
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.SQLDB.Models;

    using Microsoft.EntityFrameworkCore;

    [ExcludeFromCodeCoverage]
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CapacityDevelopment>().HasNoKey().ToView(null);
            modelBuilder.Entity<Outage>().HasNoKey().ToView(null);
        }

        public DbSet<Unit> Unit { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<RegionCC> Region { get; set; }
        public DbSet<Outage> Outage { get; set; }
        public DbSet<TableConfiguration> TableConfiguration { get; set; }
        public DbSet<CapacityDevelopment> CapacityDevelopment { get; set; }
    }
}
