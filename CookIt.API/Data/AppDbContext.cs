using CookIt.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CookIt.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Relation> Relation { get; set; }
        public DbSet<Synset> Synset { get; set; }
        public DbSet<Wordsense> Wordsense { get; set; }
        public DbSet<Word> Word { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredient { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Host> Host { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Relation>(eb =>
                {
                    eb.HasNoKey();
                });
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
                entity.Property(p => p.IngredientId).HasColumnType("UniqueIdentifier");
                entity.Property(p => p.RecipeId).HasColumnType("UniqueIdentifier");
            });
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
                entity.Property(p => p.HostId).HasColumnType("UniqueIdentifier");
            });
            modelBuilder.Entity<Host>(entity =>
            {
                entity.Property(p => p.Id).HasColumnType("UniqueIdentifier").HasDefaultValueSql("NEWID()");
            });
        }
    }
}