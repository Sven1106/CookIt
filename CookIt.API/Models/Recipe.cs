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
        public Uri ImageUrl { get; set; }
        public List<RecipeIngredient> RecipeIngredients { get; set; }
        public Recipe()
        {

        }
        public Recipe(string Title, Host Host, Uri Url, Uri ImageUrl)
        {
            this.Id = Guid.NewGuid();
            this.Title = Title;
            this.Host = Host;
            this.Url = Url;
            this.ImageUrl = ImageUrl;
        }
    }
    public class RecipeFilter
    {
        public List<Guid> IngredientsIds { get; set; }
    }
}
