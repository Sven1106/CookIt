using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public partial class CreateRecipeDto
    {
        public string ProjectName { get; set; }
        public Uri Domain { get; set; }
        public TasksDTO Tasks { get; set; }
    }
    public partial class TasksDTO
    {
        public List<AllRecipes> AllRecipes { get; set; }
    }
    public partial class AllRecipes
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
        public string Src { get; set; }
        public string Alt { get; set; }
    }
    public partial class MetadataDTO
    {
        public Uri FoundAtUrl { get; set; }
        public DateTimeOffset DateFound { get; set; }
    }
}
