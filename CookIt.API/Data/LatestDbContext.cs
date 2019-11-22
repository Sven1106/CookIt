using CookIt.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CookIt.API.Data {
    public class LatestDbContext : DbContext
    {
        public LatestDbContext(DbContextOptions<LatestDbContext> options) : base(options) { }
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
            modelBuilder
                .Entity<Relation>(eb =>
                {
                    eb.HasNoKey();
                });
        }
    }
}