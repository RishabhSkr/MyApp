using BackendAPI.Dtos.RawMaterial;

namespace BackendAPI.Services.RawMaterial
{
    public interface IRawMaterialService
    {
        Task<IEnumerable<RawMaterialResponseDto>> GetAllAsync();
        Task<string> CreateRawMaterialAsync(RawMaterialCreateDto dto, int userId);
        Task<string> UpdateRawMaterialAsync(int id,RawMaterialUpdateDto dto, int userId);
        Task<string> AddStockAsync(StockUpdateDto dto, int userId);
    }
}