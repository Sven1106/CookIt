using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Recipe
    {

        public Guid Id { get; set; }
        public string Title { get; set; }
        public Host Host { get; set; }
        public Uri Url { get; set; }
        public string ImageUrl { get; set; }
        public List<RecipeSentence> RecipeSentences { get; set; }
        
        public Recipe()
        {

        }
        public Recipe(string title, Host host, Uri url, string imageUrl)
        {
            this.Id = Guid.NewGuid();
            this.Title = title;
            this.Host = host;
            this.Url = url;
            this.ImageUrl = imageUrl.ToString();
        }
    }
    public class GetRecipesFilterDto
    {
        [Required]
        public List<Guid> IngredientsIds { get; set; }
        public List<Guid> HostIds { get; set; }
        public int? MissingIngredientsLimit { get; set; }
    }
}
