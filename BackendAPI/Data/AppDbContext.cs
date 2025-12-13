// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;

namespace BackendAPI.Data
{
    // This class inherits from DbContext (built-in .NET tool)
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // This tells .NET: "I want a table called 'Products' based on the 'Product' class"
        public DbSet<Product> Products { get; set; }
        
  
    }
}