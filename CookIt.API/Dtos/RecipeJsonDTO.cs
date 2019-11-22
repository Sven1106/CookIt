using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public partial class RecipeJsonDto
    {
        public string ProjectName { get; set; }
        public Uri StartUrl { get; set; }
        public Data Data { get; set; }
    }

    public partial class Data
    {
        public List<AllRecipe> AllRecipes { get; set; }
    }

    public partial class AllRecipe
    {
        public Recipe Recipe { get; set; }
        public Metadata Metadata { get; set; }
    }

    public partial class Metadata
    {
        public Uri FoundAtUrl { get; set; }
        public DateTimeOffset DateFound { get; set; }
    }

    public partial class Recipe
    {
        public string Heading { get; set; }
        public List<string> Ingredients { get; set; }
        public Image Image { get; set; }
    }

    public partial class Image
    {
        public Uri Src { get; set; }
        public string Alt { get; set; }
    }

}
