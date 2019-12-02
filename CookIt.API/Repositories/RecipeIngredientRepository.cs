using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Repositories
{
    public class RecipeIngredientRepository : BaseRepository<RecipeIngredient>, IRecipeIngredientRepository
    {
        private readonly AppDbContext appDbContext;
        public RecipeIngredientRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }
    }
}
