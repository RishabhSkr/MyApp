
using ProductEntity = BackendAPI.Models.Product;
namespace BackendAPI.Repositories.ProductRepository
{
    public interface IProductRepository
    {
        // Return type IEnumerable rakhein (List nahi)
        Task<IEnumerable<ProductEntity>> GetAllAsync(); 
        Task<ProductEntity?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        
        Task<ProductEntity> AddAsync(ProductEntity product);
        Task UpdateAsync(ProductEntity product);
        Task DeleteAsync(int id);
    }
}