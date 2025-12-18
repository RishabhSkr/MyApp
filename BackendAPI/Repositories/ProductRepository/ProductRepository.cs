using BackendAPI.Data;
using Microsoft.EntityFrameworkCore;
using ProductEntity = BackendAPI.Models.Product;

namespace BackendAPI.Repositories.ProductRepository;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Products.AnyAsync(p => p.Name == name && p.IsActive);
    }

    // public async Task<List<Product>> GetAllAsync()
    // {
    //     return await _context.Products.ToListAsync();
    // }
    public async Task<IEnumerable<ProductEntity>> GetAllAsync()
    {
        // Sirf Active records lao
        return await _context.Products
                             .Where(p => p.IsActive == true) 
                             .Include(p => p.CreatedByUser)
                             .ToListAsync();
    }
    public async Task<ProductEntity?> GetByIdAsync(int id)
    {
        // Check Active status
        return await _context.Products
                             .Include(p => p.CreatedByUser)
                             .FirstOrDefaultAsync(p => p.ProductId == id && p.IsActive);
    }

    public async Task<ProductEntity> AddAsync(ProductEntity product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }
    public async Task UpdateAsync(ProductEntity product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        //  Soft Delete
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.IsActive = false; // Asli Delete nahi, bas flag change
            await _context.SaveChangesAsync();
        }
    }
}
