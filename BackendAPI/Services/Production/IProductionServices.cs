using BackendAPI.Dtos.Production;

namespace BackendAPI.Services.Production
{
    public interface IProductionService
    {
        Task<IEnumerable<PendingOrderDto>> GetPendingSalesOrdersAsync();
        Task<ProductionPlanningInfoDto> GetPlanningInfoAsync(Guid salesOrderId);
        Task<IEnumerable<ProductionOrderListDto>> GetAllProductionOrdersAsync(Guid? salesOrderId = null);
        Task<string> CreateProductionPlanAsync(CreateProductionDto dto, Guid userId);
        Task<string> StartProductionAsync(Guid poId, Guid userId);
        // Task<string> CompleteProductionAsync(Guid productionOrderId, Guid userId);

        Task<string> CompleteProductionAsync(CompleteProductionDto dto, Guid userId);
        Task<string> CancelProductionOrderAsync(Guid productionOrderId, Guid userId);
    }
}