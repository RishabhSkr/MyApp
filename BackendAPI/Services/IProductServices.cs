using BackendAPI.Models;
using BackendAPI.Dtos.Product;
namespace BackendAPI.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<Product> CreateAsync(ProductCreateDto product);
    Task UpdateAsync(int id,ProductUpdateDto product);
    Task DeleteAsync(int id);
}
