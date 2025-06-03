﻿using BytagrammAPI.Data;
using BytagrammAPI.Models;
using BytagrammAPI.Repositories.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BytagrammAPI.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) : base(context)
        {
        }

        public override async Task<User> GetByIdAsync(string id)
        {
            return await GetAllAsQueryable().Include(u => u.SubscribedCommunities).FirstAsync(u =>  u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await GetAllAsQueryable().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await GetAllAsQueryable().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await GetAllAsQueryable().FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<bool> VerifyPasswordAsync(User user, string password)
        {
            if (user == null || string.IsNullOrWhiteSpace(password))
                return false;

            // Compute the hash of the provided password
            var hashedPassword = ComputeSha256Hash(password);

            // Compare the hashed password with the stored password hash
            return user.PasswordHash.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}