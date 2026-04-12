using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using System.Net.Http.Headers;

namespace Shop.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
