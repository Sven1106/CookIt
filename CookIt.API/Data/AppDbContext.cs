using CookIt.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CookIt.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<Host> Host { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<RecipeSentence> RecipeSentence { get; set; }
        public DbSet<RecipeSentenceIngredient> RecipeSentenceIngredient { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<Host>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
            
            modelBuilder.Entity<RecipeSentence>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<RecipeSentenceIngredient>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
        }
    }
}