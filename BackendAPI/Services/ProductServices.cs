using BackendAPI.Models;
using BackendAPI.Repositories;

namespace BackendAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Product>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<Product?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public async Task<Product> CreateAsync(ProductCreateDtos dto)
    {
        var Product = new ProductService
        {
            Name = dto.Name,
            Price = dto.Price
        };
       return  await _repository.AddAsync(Product);
    }

    public Task UpdateAsync(Product product)
        => _repository.UpdateAsync(product);

    public Task DeleteAsync(int id)
        => _repository.DeleteAsync(id);
}
