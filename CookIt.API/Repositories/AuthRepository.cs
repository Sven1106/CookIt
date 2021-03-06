﻿using CookIt.API.Data;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CookIt.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appDbContext;
        public AuthRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _appDbContext.User.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.Id = Guid.NewGuid();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _appDbContext.User.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            if (await _appDbContext.User.AnyAsync(x => x.Email == email))
            {
                return true;
            }
            return false;
        }
    }
}
