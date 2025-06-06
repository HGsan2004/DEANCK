using QLCHNT.Const;
using QLCHNT.Dto.Order;

namespace QLCHNT.Service.Order
{
    public interface IOrderService
    {
        // gửi user Id 
        // Tìm cart theo usr id > chưa thì lỗi chưa có giior hàng 
        // kiểm tra cart.item nếu = 0 thì lỗi chưa thêm sản phẩm 
        // tạo orderentity : OrderDat = now, Status=>pàning, 
        // Tạo 1 líst orderitementity (orderid = entity.id ) từ cartitrmentity => map => items
        // Tạo entity 
        Task<OrderDto> CreateAsync(OrderCreateRequest request);
        Task<List<OrderDto>> GetAllAsync();
        Task<List<OrderDto>> GetByUserAsync(Guid userId);
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<OrderDto?> UpdateStatusAsync(Guid id, Enums.Status status);
        Task<bool> DeleteAsync(Guid id);
        Task<RevenueResultDto> GetRevenueStatisticAsync(RevenueRequestDto request);
        Task<PagedResult<OrderDto>> GetPagedOrdersAsync(OrderGetPageingRequest request);
        
        // thống kê doanh thu:
        // Nhận vào: dto(startDate, endDate) => thống kê 14/5/2024 - 20/5/2024
        // Lấy tất cả order có OrderDate nằm trong khoảng trên >= start, <= end => Map orderEntity sang OrderDto có TotalPrice
        // SUM TotalPrice => Price
        // SUM số lượng sản phẩm được bán theo danh mục, ví dụ: Ghế: 30, Bàn: 10, ...
        
    }
}
