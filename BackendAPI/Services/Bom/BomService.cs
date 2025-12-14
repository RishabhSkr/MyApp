using AutoMapper;
using BackendAPI.Dtos.Bom;
using BackendAPI.Repositories.Bom;
using BackendAPI.Data;
using BackendAPI.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

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

        // get by ProductID
        public async Task<BomResponseDto?> GetBomByProductAsync(int productId)
        {
            var bomList = await _repo.GetByProductIdAsync(productId);

            if (!bomList.Any())
                throw new NotFoundException("BOM not found");

            var first = bomList.First();

            var response = new BomResponseDto
            {
                ProductId = first.ProductID,
                ProductName = first.Product!.Name,
                BomItems = bomList.Select(b => new BomItemDto
                {
                    RawMaterialId = b.RawMaterialId,
                    RawMaterialName = b.RawMaterial!.Name,
                    QuantityRequired = b.QuantityRequired
                }).ToList()
            };

            return response;
        }


        public async Task<string> CreateBomAsync(BomCreateDto dto)
        {
            // Transaction Start 
            using var transaction = await _context.Database.BeginTransactionAsync();
            return "Success";
        }
    }
}