using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CookIt.API.Dtos;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CookIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _iConfig;
        private readonly IAuthRepository _authRepository;

        public AuthController(IConfiguration iConfig, IAuthRepository authRepository)
        {
            this._iConfig = iConfig;
            this._authRepository = authRepository;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserForRegisterDto userForRegisterDto)
        {
            if (userForRegisterDto.Name == null || userForRegisterDto.Email == null || userForRegisterDto.Password == null)
            {
                return BadRequest("Invalid Dto");
            }
            userForRegisterDto.Email = userForRegisterDto.Email.ToLower();
            if (await _authRepository.UserExistsAsync(userForRegisterDto.Email))
            {
                return BadRequest("Email already exists");
            }
            User userToCreate = new User()
            {
                Name = userForRegisterDto.Name,
                Email = userForRegisterDto.Email,
                Role = Role.User
            };

            User createdUser = await _authRepository.RegisterAsync(userToCreate, userForRegisterDto.Password);
            if (createdUser == null)
            {
                return StatusCode(500, "Register failed");
            }
            string userToken = CreateUserToken(createdUser.Id.ToString(), createdUser.Name, createdUser.Email, createdUser.Role);
            return Ok(new
            {
                token = userToken
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserForLoginDto userForLoginDto)
        {
            if (userForLoginDto.Email == null || userForLoginDto.Password == null)
            {
                return BadRequest("Invalid Dto");
            }
            userForLoginDto.Email = userForLoginDto.Email.ToLower();
            var user = await _authRepository.LoginAsync(userForLoginDto.Email, userForLoginDto.Password);
            if (user == null)
            {
                return BadRequest("Email or password is wrong");
            }
            string userToken = CreateUserToken(user.Id.ToString(), user.Name, user.Email, user.Role);

            return Ok(new
            {
                token = userToken
            });
        }

        private string CreateUserToken(string id, string name, string email, string role)
        {

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role,role)
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
            return jwtTokenHandler.WriteToken(createdToken);
        }


    }
}