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

namespace CookIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        [HttpGet("getMyIngredients")]
        public async Task<IActionResult> GetMyIngredients()
        {
            //if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out Guid userId))
            //{


            //    ingredient = await _appDbContext.Ingredient.Where(x => x.Id == ingredientId).FirstOrDefaultAsync();
            //}
            return Ok();
        }
    }
}