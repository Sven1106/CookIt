using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using CookIt.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IRecipeRepository RecipeRepo { get; }
        public IHostRepository HostRepo { get; }
        public IIngredientRepository IngredientRepo { get; }
        public IRecipeSentenceRepository RecipeSentenceRepo { get; }
        public IRecipeSentenceIngredientRepository RecipeSentenceIngredientRepo { get; }


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            RecipeRepo = new RecipeRepository(_context);
            HostRepo = new HostRepository(_context);
            IngredientRepo = new IngredientRepository(_context);
            RecipeSentenceRepo = new RecipeSentenceRepository(_context);
            RecipeSentenceIngredientRepo = new RecipeSentenceIngredientRepository(_context);
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
