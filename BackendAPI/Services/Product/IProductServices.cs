using BackendAPI.Dtos.Product;

namespace BackendAPI.Services.Product
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto?> GetByIdAsync(int id);
        
        Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, int userId);
        
        Task UpdateAsync(int id, ProductUpdateDto dto, int userId);
        
        Task DeleteAsync(int id);
    }
}