using AutoMapper;
using BackendAPI.Dtos.Bom;
using BackendAPI.Repositories.Bom;
using BackendAPI.Data;
using BackendAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services.Bom
{
    public class BomService : IBomService
    {
        private readonly IBomRepository _repo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context; // Transaction ke liye

        public BomService(IBomRepository repo, IMapper mapper, AppDbContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
        }

        // get by ProductId
        public async Task<BomResponseDto?> GetBomByProductAsync(int productId)
        {
            var bomList = await _repo.GetByProductIdAsync(productId);
            Console.WriteLine(bomList);
            if (!bomList.Any())
                throw new NotFoundException("BOM not found");

            var first = bomList.First();

            var response = new BomResponseDto
            {
                productId = first.ProductId,
                productName = first.Product!.Name,
                BomItems = _mapper.Map<List<BomResponseItemDto>>(bomList)
            };
            return response;
        }


        public async Task<string> CreateBomAsync(BomCreateDto dto)
        {
            // 1. Safety Lock (Transaction)
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 2. Validate Product (Kya Product exist karta hai?)
                var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.productId);
                if (!productExists)
                    return "Error: Product ID invalid.";

                // 3. Validate Duplicate (Kya BOM already hai? Agar haan, toh error do ya update logic lagao)
                var bomExists = await _repo.ExistsAsync(dto.productId);
                if (bomExists)
                    return "Error: BOM already exists for this product. Use Update API.";

                // 4. List taiyaar karo Database ke liye
                var bomEntities = new List<BackendAPI.Models.Bom>();

                foreach (var item in dto.BomItems)
                {
                    // Optional: Check karo Raw Material exist karta hai ya nahi
                    var rmExists = await _context.RawMaterials.AnyAsync(r => r.RawMaterialId == item.RawMaterialId);
                    if (!rmExists) 
                        return $"Error: Raw Material ID {item.RawMaterialId} not found.";

                    // 5. MAPPING (DTO -> Entity)
                    // DTO se data nikal kar Model mein bhar rahe hain
                    bomEntities.Add(new BackendAPI.Models.Bom
                    {
                        ProductId = dto.productId,
                        RawMaterialId = item.RawMaterialId,
                        QuantityRequired = item.QuantityRequired,
                        CreatedByUserId = 1 // Abhi ke liye hardcode, baad mein JWT se nikalenge
                    });
                }

                // 6. Save to DB (Bulk Insert via Repository)
                await _repo.AddRangeAsync(bomEntities);

                // 7. Sab sahi hai, commit karo
                await transaction.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                // Kuch galti hui toh wapas undo karo
                await transaction.RollbackAsync();
                return $"Error: {ex.Message}";
            }
        }
    }
}