using BackendAPI.Dtos.Production;

namespace BackendAPI.Services.Production
{
    public interface IProductionService
    {
        Task<string> CreateProductionPlanAsync(CreateProductionDto dto, int userId);
    }
}