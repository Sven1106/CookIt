using CookIt.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CookIt.API.Data {
    public class DataContext : DbContext {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Relation> Relations { get; set; }
        public DbSet<Synset> Synsets { get; set; }
        public DbSet<Wordsense> Wordsenses { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Recipe> Recipes{ get; set; }
        public DbSet<Host> Hosts { get; set; }


    }
}