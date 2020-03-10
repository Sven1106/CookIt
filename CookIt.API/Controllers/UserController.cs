using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using CookIt.API.Dtos;

namespace CookIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        public UserController(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }
        [HttpPost("addRecipeToFavorite/{recipeId}")]
        public async Task<IActionResult> AddRecipeToFavorite(Guid recipeId)
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out Guid userId))
            {
                Recipe recipe = await _recipeRepository.GetRecipeAsync(recipeId);
                if (recipe == null)
                {
                    return BadRequest("No recipe found");
                }
                int changesMade = await _recipeRepository.CreateFavoriteRecipeAsync(userId, recipe);
                if (changesMade == 0)
                {
                    return BadRequest("No changes were made");
                }
            }
            else
            {
                return StatusCode(500, "AddRecipeToFavorite failed");
            }
            return Ok();
        }

        [HttpGet("getAllFavoriteRecipes")]
        public async Task<IActionResult> GetAllFavoriteRecipes()
        {
            List<FavoriteRecipeDto> favoriteRecipes = new List<FavoriteRecipeDto>();
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out Guid userId))
            {
                favoriteRecipes = await _recipeRepository.GetFavoriteRecipes(userId);
                if (favoriteRecipes == null)
                {
                    return StatusCode(500, "GetFavoriteRecipes failed");
                }
                if (favoriteRecipes.Count == 0)
                {
                    return NoContent();
                }
            }
            else
            {
                return StatusCode(500, "No userId found in claims");
            }
            return Ok(favoriteRecipes);
        }
    }
}