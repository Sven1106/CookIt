using CookIt.API.Core;
using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Interfaces
{
    public interface IAuthRepository
    {
        User Register(User user, string password);
        User Login(string username, string password);
        bool UserExists(string username);
    }
}
