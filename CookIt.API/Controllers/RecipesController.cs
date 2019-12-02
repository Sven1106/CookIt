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
        public async Task<ActionResult> CreateRecipes(CreateRecipeDto json)
        {
            _unitOfWorkManager.CreateRecipes(json);
            return Ok("");
        }
        [HttpGet]
        public async Task<IActionResult> GetRecipes()
        {
            return Ok("");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipe(Guid id)
        {
            return Ok();
        }
    }
}
