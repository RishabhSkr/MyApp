using BackendAPI.Models;
using BackendAPI.Repositories;
using BackendAPI.Dtos;
using AutoMapper;
namespace BackendAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<List<Product>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<Product?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public async Task<Product> CreateAsync(ProductCreateDto dto)
    {   
        // manual mapping
        // var Product = new ProductService
        // {
        //     Name = dto.Name,
        //     Price = dto.Price
        // };

        // automapper
        var product = _mapper.Map<Product>(dto);
        return await _repository.AddAsync(product);
    }

    public async Task UpdateAsync(int id,ProductUpdateDto dto)
    {
        var productToUpdate = await _repository.GetByIdAsync(id);
        if (productToUpdate == null)
            throw new Exception("Product not found");
        _mapper.Map(dto, productToUpdate);
        await _repository.UpdateAsync(productToUpdate);
    }

    public Task DeleteAsync(int id)
        => _repository.DeleteAsync(id);
}
