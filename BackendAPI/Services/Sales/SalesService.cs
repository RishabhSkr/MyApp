using SalesEntity = BackendAPI.Models.SalesOrder;
using BackendAPI.Repositories.SalesRepository;
using BackendAPI.Dtos.Sales;
using AutoMapper;
using BackendAPI.Exceptions;
using BackendAPI.Services.Sales;


public class SalesService : ISalesService
{
    private readonly ISalesRepository _repository;
    private readonly IMapper _mapper;

    public SalesService(ISalesRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SalesResponseDto>> GetPendingSalesOrdersAsync()
    {
        var salesOrders = await _repository.GetOrdersByStatusAsync("Pending");

        Console.WriteLine($"Fetched {salesOrders.Count()} pending orders from DB.");

        return _mapper.Map<IEnumerable<SalesResponseDto>>(salesOrders);
    }
}