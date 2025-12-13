using BackendAPI.Models;
using BackendAPI.Dtos;
namespace BackendAPI.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(ProductCreateDto product);
    Task UpdateAsync(int id,ProductUpdateDto product);
    Task DeleteAsync(int id);
}
