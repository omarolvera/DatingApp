using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int Id)
        {
          var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == Id);
            return photo;
        }


        public async Task<User> GetUser(int Id)
        {
            var user = await _context.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u=> u.Id == Id);
            return user;
        }

       // public async Task<IEnumerable<User>> GetUsers()
       public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            //var users = await _context.Users.Include(p=> p.Photos).ToListAsync();
            var users = _context.Users.Include(p => p.Photos)
            .OrderByDescending(o => o.LastActive).AsQueryable();

            //adding filter for userid and gender
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);

            //add filter by min and max age
            if(userParams.MinAge != 18 || userParams.MaxAge != 99){
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge);
                var maxAge = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxAge);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy)){
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(o=> o.Created);
                        break;
                    default:
                        users = users.OrderByDescending(o=> o.LastActive);
                        break;
                }  
            }

            //return users;
            return await PagedList<User>.CreateAsync(users,userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}