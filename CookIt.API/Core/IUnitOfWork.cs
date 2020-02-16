using CookIt.API.Data;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Core
{
    public interface IUnitOfWork : IDisposable
    {
        #region Add all repositoryInterfaces here.
        IHostRepository HostRepo { get; }
        IRecipeRepository RecipeRepo { get; }
        IIngredientRepository IngredientRepo { get; }
        IRecipeSentenceRepository RecipeSentenceRepo { get; }
        IRecipeSentenceIngredientRepository RecipeSentenceIngredientRepo { get; }

        #endregion
        int Complete();
    }
}
