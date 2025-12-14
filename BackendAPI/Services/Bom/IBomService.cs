using BackendAPI.Dtos.Bom;

namespace BackendAPI.Services.Bom
{
    public interface IBomService
    {
        Task<BomResponseDto?> GetBomByProductAsync(int productId);
        Task<string> CreateBomAsync(BomCreateDto dto); // Returns Success/Error Message
    }
}