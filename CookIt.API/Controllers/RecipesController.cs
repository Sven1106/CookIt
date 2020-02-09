using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Dtos;
using CookIt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Recipe = CookIt.API.Models.Recipe;

namespace CookIt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly UnitOfWorkManager _unitOfWorkManager;
        public RecipesController(IUnitOfWork unitOfWork)
        {
            _unitOfWorkManager = new UnitOfWorkManager(unitOfWork);
        }
        [HttpPost]
        public async Task<ActionResult> CreateRecipesAsync(CreateRecipeDto json)
        {
            int rowsAdded = await _unitOfWorkManager.CreateRecipesAsync(json);
            return Ok("CreateRecipesAsync DONE");
        }
        [HttpGet]
        public async Task<IActionResult> GetRecipesAsync([FromQuery]RecipeFilter filter)
        {
            List<Recipe> recipes = await _unitOfWorkManager.GetRecipesAsync(filter);
            if (recipes == null)
            {
                return NoContent();
            }
            return Ok(recipes);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeAsync(Guid id)
        {
            Recipe recipe = await _unitOfWorkManager.GetRecipeAsync(id);
            if (recipe == null)
            {
                return NoContent();
            }
            return Ok(recipe);
        }
    }
}
