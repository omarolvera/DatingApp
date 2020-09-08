using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        //TODO: create datacontext inject from Nuget Microsoft.EntityFramework.core
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
     
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
    }
}