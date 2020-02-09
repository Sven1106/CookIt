using CookIt.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CookIt.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredient { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Host> Host { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Ingredient>().Ignore(c => c.Lemmas);
            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<Host>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
        }
    }
}