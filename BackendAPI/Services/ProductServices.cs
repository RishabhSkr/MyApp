using BackendAPI.Models;
using BackendAPI.Repositories;
using BackendAPI.Dtos;
using AutoMapper;
using BackendAPI.Exceptions;
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

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) 
            throw new NotFoundException("Product not found");

        return _mapper.Map<ProductResponseDto>(product);
    }

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
