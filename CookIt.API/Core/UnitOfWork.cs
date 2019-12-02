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
        public IWordRepository WordRepo { get; }
        public IIngredientRepository IngredientRepo { get; }
        public IRecipeIngredientRepository RecipeIngredientRepo { get; }


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            RecipeRepo = new RecipeRepository(_context);
            HostRepo = new HostRepository(_context);
            WordRepo = new WordRepository(_context);
            IngredientRepo = new IngredientRepository(_context);
            RecipeIngredientRepo = new RecipeIngredientRepository(_context);
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
