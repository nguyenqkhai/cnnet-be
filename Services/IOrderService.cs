using ElearningBackend.DTOs;

namespace ElearningBackend.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrdersByUserIdAsync(long userId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<List<OrderDto>> GetAllOrdersForAdminAsync();
        Task<OrderDto> GetOrderByIdAsync(long id);
        Task<OrderDto> UpdateOrderAsync(long id, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(long id);
    }
}
