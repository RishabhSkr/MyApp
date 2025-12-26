using BackendAPI.Dtos.Production;

namespace BackendAPI.Services.Production
{
    public interface IProductionService
    {
        Task<IEnumerable<PendingOrderDto>> GetPendingSalesOrdersAsync();
        Task<ProductionPlanningInfoDto> GetPlanningInfoAsync(int salesOrderId);
        Task<IEnumerable<ProductionOrderListDto>> GetAllProductionOrdersAsync(int? salesOrderId = null);
        Task<string> CreateProductionPlanAsync(CreateProductionDto dto, int userId);
        Task<string> StartProductionAsync(int poId, int userId);
        // Task<string> CompleteProductionAsync(int productionOrderId, int userId);

        Task<string> CompleteProductionAsync(CompleteProductionDto dto, int userId);
        Task<string> CancelProductionOrderAsync(int productionOrderId, int userId);
    }
}