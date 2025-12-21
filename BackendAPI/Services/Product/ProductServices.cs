using ProductEntity = BackendAPI.Models.Product;
using BackendAPI.Repositories.ProductRepository;
using BackendAPI.Dtos.Product;
using AutoMapper;
using BackendAPI.Exceptions;


namespace BackendAPI.Services.Product;

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

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, int userId)
    {
        // 1. Validation
        if (await _repository.ExistsByNameAsync(dto.Name))
            throw new BadRequestException("Product already exists.");

        // 2. Map DTO to Entity
        var product = _mapper.Map<ProductEntity>(dto);

        // 3. SET AUDIT FIELD (Manual Step)
        // CreatedAt automatic hai (DbContext karega), lekin UserId humein dena padega
        product.CreatedByUserId = userId; 

        // 4. Save
        var createdProduct = await _repository.AddAsync(product);
        return _mapper.Map<ProductResponseDto>(createdProduct);
    }

    public async Task UpdateAsync(int id, ProductUpdateDto dto, int userId)
    {   var product = await _repository.GetByIdAsync(id);
        if (product == null) throw new NotFoundException("Product not found");

            // Agar naam change ho raha hai to duplicate check
            var alreadyNameExists = await _repository.ExistsByNameAsync(dto.Name);
            if (product.Name != dto.Name && alreadyNameExists)
                throw new BadRequestException("Product name already taken.");

            _mapper.Map(dto, product);

            // Set Audit Field
            product.UpdatedByUserId = userId;

            await _repository.UpdateAsync(product);
    }

    public Task DeleteAsync(int id)
        => _repository.DeleteAsync(id);
}
