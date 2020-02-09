using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public partial class CreateRecipeDto
    {
        public string ProjectName { get; set; }
        public Uri StartUrl { get; set; }
        public TaskDTO Task { get; set; }
    }
    public partial class TaskDTO
    {
        public List<AllRecipesDTO> AllRecipes { get; set; }
    }
    public partial class AllRecipesDTO
    {
        public RecipeDTO Recipe { get; set; }
        public MetadataDTO Metadata { get; set; }
    }
    public partial class RecipeDTO
    {
        public string Heading { get; set; }
        public List<string> Ingredients { get; set; }
        public ImageDTO Image { get; set; }
    }
    public partial class ImageDTO
    {
        public Uri Src { get; set; }
        public string Alt { get; set; }
    }
    public partial class MetadataDTO
    {
        public Uri FoundAtUrl { get; set; }
        public DateTimeOffset DateFound { get; set; }
    }
}
