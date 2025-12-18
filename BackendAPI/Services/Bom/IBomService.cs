using BackendAPI.Dtos.Bom;

namespace BackendAPI.Services.Bom
{
    public interface IBomService
    {
        Task<BomResponseDto?> GetBomByProductAsync(int productId);
        Task<IEnumerable<BomResponseDto>> GetAllBomsAsync(); 
        Task<string> CreateBomAsync(BomCreateDto dto, int userId);
        Task<string> UpdateBomAsync(int productId, BomCreateDto dto, int userId);
        Task<string> DeleteBomAsync(int productId);
    }
}