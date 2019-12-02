using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public partial class CreateRecipeDto
    {
        public string ProjectName { get; set; }
        public string StartUrl { get; set; }
        public DataDTO Data { get; set; }
    }
    public partial class DataDTO
    {
        public List<AllRecipeDTO> AllRecipes { get; set; }
    }
    public partial class AllRecipeDTO
    {
        public RecipeDTO Recipe { get; set; }
        public MetadataDTO Metadata { get; set; }
    }
    public partial class MetadataDTO
    {
        public string FoundAtUrl { get; set; }
        public DateTimeOffset DateFound { get; set; }
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
}
