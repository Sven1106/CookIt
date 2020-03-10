using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class FavoriteRecipe
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RecipeId { get; set; }
        public FavoriteRecipe()
        {

        }
        //public FavoriteRecipe(User user,Recipe recipe)
        //{
        //    this.User = user;
        //    this.Recipe = recipe;
        //}

        public FavoriteRecipe(Guid userId, Guid recipeId)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.RecipeId = recipeId;
        }
    }
}
