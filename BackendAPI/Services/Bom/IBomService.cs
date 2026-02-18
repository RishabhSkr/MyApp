using BackendAPI.Dtos.Bom;

namespace BackendAPI.Services.Bom
{
    public interface IBomService
    {
        Task<BomResponseDto?> GetBomByProductAsync(Guid productId);
        Task<IEnumerable<BomResponseDto>> GetAllBomsAsync(); 
        Task<string> CreateBomAsync(BomCreateDto dto, Guid userId);
        Task<string> UpdateBomAsync(Guid productId, BomCreateDto dto, Guid userId);
        Task<string> DeleteBomAsync(Guid productId);
    }
}