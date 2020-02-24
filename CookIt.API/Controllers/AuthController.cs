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
        public async Task<IActionResult> RegisterAsync(UserForRegisterDto userForRegisterDTO)
        {
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();
            if (await _authRepository.UserExistsAsync(userForRegisterDTO.Username))
            {
                return BadRequest("Username already exists");
            }
            User userToCreate = new User()
            {
                Username = userForRegisterDTO.Username,
                Role = Role.User
            };

            await _authRepository.RegisterAsync(userToCreate, userForRegisterDTO.Password);
            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserForLoginDto userForLoginDto)
        {
            userForLoginDto.Username = userForLoginDto.Username.ToLower();
            var user = await _authRepository.LoginAsync(userForLoginDto.Username, userForLoginDto.Password);
            if (user == null)
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
            return Ok(new
            {
                token = jwtTokenHandler.WriteToken(createdToken)
            });
        }
    }
}