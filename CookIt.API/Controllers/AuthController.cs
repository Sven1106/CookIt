using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Core;
using CookIt.API.Dtos;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CookIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UnitOfWorkManager _unitOfWorkManager;

        public AuthController( IUnitOfWork unitOfWork)
        {
            this._unitOfWorkManager = new UnitOfWorkManager(unitOfWork);
        }
        [HttpPost("register")]
        public IActionResult Register(UserForRegisterDTO userForRegisterDTO)
        {
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();
            if (_unitOfWorkManager.UserExists(userForRegisterDTO.Username))
            {
                return BadRequest("Username already exists");
            }
            User userToCreate = new User
            {
                Username = userForRegisterDTO.Username
            };

            _unitOfWorkManager.Register(userToCreate, userForRegisterDTO.Password);
            return StatusCode(201);
        }
    }
}