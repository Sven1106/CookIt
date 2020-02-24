using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using CookIt.API.Areas.Admin.Models;
using CookIt.API.Core;
using CookIt.API.Dtos;
using CookIt.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace CookIt.API.Areas.Admin.Controllers
{
    //[LoginAuthorize("Role", Role.Admin)]
    [Authorize] // https://github.com/shawnwildermuth/dualauthcore
    [Area("Admin")]
    public class CMSController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _iConfig;
        private string _currentDirectory;
        private JSchema _createRecipeSchema;
        public CMSController(IAuthRepository authRepository, IIngredientRepository ingredientRepository, IRecipeRepository recipeRepository, IConfiguration iConfig)
        {
            _authRepository = authRepository;
            _ingredientRepository = ingredientRepository;
            _recipeRepository = recipeRepository;
            _iConfig = iConfig;
            _currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _createRecipeSchema = JSchema.Parse(System.IO.File.ReadAllText(Path.Combine(_currentDirectory, "createRecipeSchema.json")));
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost, AllowAnonymous]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            userForLoginDto.Username = userForLoginDto.Username.ToLower();
            var user = _authRepository.Login(userForLoginDto.Username, userForLoginDto.Password);
            if (user == null || user.Role != Role.Admin)
            {
                return Unauthorized();
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._iConfig.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var createdToken = jwtTokenHandler.CreateToken(tokenDescriptor);

            HttpContext.Session.SetString("JWT", jwtTokenHandler.WriteToken(createdToken));
            return RedirectToAction("Index");
        }
        public ActionResult Signout()
        {
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
        public IActionResult CreateRecipes(string json)
        {
            JSchemaValidatingReader jSchemaReader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            jSchemaReader.Schema = _createRecipeSchema;

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
            int rowsAdded = _recipeRepository.CreateRecipes(createRecipeDto);
            return RedirectToAction("Recipes");
        }

        public IActionResult Recipes()
        {
            RecipesVm recipesVm = new RecipesVm(_recipeRepository.GetRecipes(), _ingredientRepository.GetIngredients());
            return View(recipesVm);
        }
        public IActionResult DeleteRecipe(Guid id)
        {
            _recipeRepository.DeleteRecipe(id);
            return RedirectToAction("Recipes");
        }


    }
}