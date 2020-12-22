using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private  DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);
            
            if(user == null)
            return null;

            if(!verifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            return null;

            return  user;
        }
         private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
               
               var  computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
              return true;
        }

        public async Task<User> Register(User userInfo, string password)
        {
            byte[] passHash, passSalt;
            CreatePasswordHash(password, out passHash, out passSalt);
            userInfo.PasswordHash = passHash;
            userInfo.PasswordSalt = passSalt;
            
            await _context.Users.AddAsync(userInfo);
            await _context.SaveChangesAsync();

            return userInfo;
        }

        private void CreatePasswordHash(string password, out byte[] passHash, out byte[] passSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()) 
            {
                passSalt = hmac.Key;
                passHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username))
            return true;

            return false;
        }
    }
}