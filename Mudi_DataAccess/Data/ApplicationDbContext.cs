using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mudi_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mudi_DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            if (Database.IsSqlite())
            {
                // Ubuntu 20.04 ships SQLite 3.31, before RETURNING support (3.35).
                foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(entity => !entity.IsOwned() && entity.FindPrimaryKey() != null).ToList())
                    modelBuilder.Entity(entityType.ClrType).ToTable(table => table.UseSqlReturningClause(false));
            }
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<WishListDetail> WishListDetail { get; set; }

        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<WebSiteDetail> WebSiteDetail { get; set; }
        public DbSet<Cart> Cart { get; set; }
    }
}
