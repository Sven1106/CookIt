using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Repositories
{
    public class AuthRepository : BaseRepository<User>, IAuthRepository
    {
        private readonly AppDbContext appDbContext;
        public AuthRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        //public User Login(string username, string password)
        //{
        //    var user = this.appDbContext.User.Where(x => x.Username == username).FirstOrDefault();
        //    if (user == null)
        //    {
        //        return null;
        //    }
        //    if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        //    {
        //        return null;
        //    }
        //    return user;
        //}

        //private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        //{
        //    using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        //    {
        //        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        //        for (int i = 0; i < computedHash.Length; i++)
        //        {
        //            if (computedHash[i] != passwordHash[i])
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        //public User Register(User user, string password)
        //{
        //    byte[] passwordHash, passwordSalt;
        //    CreatePasswordHash(password, out passwordHash, out passwordSalt);
        //    user.PasswordHash = passwordHash;
        //    user.PasswordSalt = passwordSalt;
        //    this.Insert(user);
        //    return user;
        //}

        //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    using(var hmac = new System.Security.Cryptography.HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //    }
            
        //}

        //public bool UserExists(string username)
        //{
        //    if (this.appDbContext.User.Any(x => x.Username == username))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
