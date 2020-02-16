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
    public class RecipeSentenceIngredientRepository : BaseRepository<RecipeSentenceIngredient>, IRecipeSentenceIngredientRepository
    {
        private readonly AppDbContext appDbContext;
        public RecipeSentenceIngredientRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }
    }
}
