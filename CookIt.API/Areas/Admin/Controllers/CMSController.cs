using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CookIt.API.Areas.Admin.Models;
using CookIt.API.Models;
using CookIt.API.Dtos;
using CookIt.API.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace CookIt.API.Areas.Admin.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "", Roles = Role.Admin)] // https://github.com/shawnwildermuth/dualauthcore
    [Area("Admin")]
    public class CmsController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IAuthRepository _authRepository;
        private readonly string _currentDirectory;
        private readonly JSchema _createRecipeSchema;
        public CmsController(IAuthRepository authRepository, IIngredientRepository ingredientRepository, IRecipeRepository recipeRepository)
        {
            _authRepository = authRepository;
            _ingredientRepository = ingredientRepository;
            _recipeRepository = recipeRepository;
            _currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _createRecipeSchema = JSchema.Parse(System.IO.File.ReadAllText(Path.Combine(_currentDirectory, "createRecipeSchema.json")));
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            userForLoginDto.Email = userForLoginDto.Email.ToLower();
            var user = await _authRepository.LoginAsync(userForLoginDto.Email, userForLoginDto.Password);
            if (user == null || user.Role != Role.Admin)
            {
                return Unauthorized();
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            string returnUrl = HttpUtility.ParseQueryString(new Uri(HttpContext.Request.Headers["Referer"]).Query).Get("ReturnUrl");
            if (returnUrl == null)
            {
                return RedirectToAction("Index");
            }
            return Redirect(returnUrl);
        }
        public async Task<ActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateRecipes()
        {
            return View(_createRecipeSchema);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRecipes(string json)
        {
            JSchemaValidatingReader jSchemaReader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)))
            {
                Schema = _createRecipeSchema
            };

            IList<string> errorMessages = new List<string>();
            jSchemaReader.ValidationEventHandler += (o, a) => errorMessages.Add(a.Message);
            JsonSerializer serializer = new JsonSerializer();
            CreateRecipeDto createRecipeDto = serializer.Deserialize<CreateRecipeDto>(jSchemaReader);
            if (errorMessages.Count > 0)
            {
                foreach (var eventMessage in errorMessages)
                {
                }
            }
            int rowsAdded = await _recipeRepository.CreateRecipesAsync(createRecipeDto);
            return RedirectToAction("Recipes");
        }

        public async Task<IActionResult> Recipes()
        {
            RecipesVm recipesVm = new RecipesVm(await _recipeRepository.GetRecipesAsync(), await _ingredientRepository.GetIngredients());
            return View(recipesVm);
        }
        public async Task<IActionResult> DeleteRecipe(Guid id)
        {
            await _recipeRepository.DeleteRecipeAsync(id);
            return RedirectToAction("Recipes");
        }


    }
}