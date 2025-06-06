using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLCHNT.Const;
using QLCHNT.Dto.Order;
using QLCHNT.Entity;
using QLCHNT.Repository;

namespace QLCHNT.Service.Order
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<OrderEntity> _orderRepo;
        private readonly IRepository<OrderDetailEntity> _orderDetailRepo;
        private readonly IRepository<CartEntity> _cartRepo;
        private readonly IRepository<CartItemEntity> _cartItemRepo;
        private readonly IRepository<UserEntity> _userRepo;
        private readonly IMapper _mapper;

        public OrderService(
            IRepository<OrderEntity> orderRepo,
            IRepository<OrderDetailEntity> orderDetailRepo,
            IRepository<CartEntity> cartRepo,
            IRepository<CartItemEntity> cartItemRepo,
            IRepository<UserEntity> userRepo,
            IMapper mapper)
        {
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _mapper = mapper;
            _userRepo = userRepo;

        }

        public async Task<OrderDto> CreateAsync(OrderCreateRequest request)
        {
            var cart = await _cartRepo.AsQueryable()
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null)
                throw new Exception("Người dùng chưa có giỏ hàng.");

            if (cart.CartItems == null || !cart.CartItems.Any())
                throw new Exception("Giỏ hàng chưa có sản phẩm.");
            
            var order = new OrderEntity
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                OrderDate = DateTime.UtcNow,
                Status = Enums.Status.Pending,
            };
            

            var orderDetails = cart.CartItems.Select(ci => new OrderDetailEntity
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = ci.ProductId,
                ProductName = ci.ProductId.ToString(),
                Quantity = ci.Quantity,
                Price = ci.Price
            }).ToList();

            order.OrderDetails = orderDetails;

            await _orderRepo.CreateAsync(order);
            
            order.User = await _userRepo.GetAsync(request.UserId);

            // Optional: Xoá cart hoặc item trong cart sau khi tạo đơn hàng
            foreach (var item in cart.CartItems.ToList())
            {
                await _cartItemRepo.DeleteAsync(item.Id);
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepo.AsQueryable()
                .Include(o => o.OrderDetails).ThenInclude(ci => ci.Product)
                .Include(c => c.User)
                .ToListAsync();

            return _mapper.Map<List<OrderDto>>(orders);
        }
        
        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _orderRepo.AsQueryable()
                .Include(o => o.OrderDetails).ThenInclude(ci => ci.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<List<OrderDto>> GetByUserAsync(Guid userId)
        {
            var orders = await _orderRepo.AsQueryable()
                .Include(o => o.OrderDetails).ThenInclude(ci => ci.Product)
                .Include(c => c.User)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<OrderDto?> UpdateStatusAsync(Guid id, Enums.Status status)
        {
            var order = await _orderRepo.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return null;

            order.Status = status;
            await _orderRepo.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _orderRepo.GetAsync(id);
            if (order == null || order.Status != Enums.Status.Pending)
                return false;

            await _orderRepo.DeleteAsync(id);
            return true;
        }

        public async Task<RevenueResultDto> GetRevenueStatisticAsync(RevenueRequestDto request)
        {
            var orders = await _orderRepo.AsQueryable()
                .Where(o => o.OrderDate >= request.StartDate && o.OrderDate <= request.EndDate)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderDto>>(orders);
            var totalRevenue = orderDtos.Sum(o => o.TotalPrice);

            var categorySales = orders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.Product.Category.Name)
                .Select(g => new CategorySalesDto()
                {
                    CategoryName = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity)
                })
                .ToList();

            var newUsersCount = await _userRepo.AsQueryable()
                .Where(u => u.CreatedDate >= request.StartDate && u.CreatedDate <= request.EndDate).ToListAsync();


            return new RevenueResultDto
            {
                TotalRevenue = totalRevenue,
                CategorySales = categorySales,
                NewUsers = newUsersCount.Count
            };
        }

        public  Task<PagedResult<OrderDto>> GetPagedOrdersAsync(OrderGetPageingRequest request)
        {
            var query = _orderRepo.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .AsQueryable();

            // 🔍 Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                request.SearchText = request.SearchText.ToLower();
                // use contains to search: id, phone, address, productName
                query = query.Where(o =>
                    o.OrderDetails.Any(od => od.Product.Name.ToLower().Contains(request.SearchText))
                    || o.Id.ToString().ToLower().Contains(request.SearchText)
                    || o.User.Address.ToLower().Contains(request.SearchText)
                    || o.User.Phone.ToString().ToLower().Contains(request.SearchText));
            }

            // 📎 Lọc theo UserId
            if (request.UserId.HasValue)
            {
                query = query.Where(o => o.UserId == request.UserId.Value);
            }

            // 📎 Lọc theo trạng thái đơn hàng
            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            // 📅 Lọc theo ngày
            if (request.StartDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= request.EndDate.Value);
            }

            // Tổng và phân trang
            var total = query.Count();
            if (request.PageIndex.HasValue && request.PageSize.HasValue)
            {
                query = query.Skip((request.PageIndex.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
            }

            var mappedItems = _mapper.Map<List<OrderDto>>(query.ToList());

            return Task.FromResult(new PagedResult<OrderDto>
            {
                TotalItems = total,
                Items = mappedItems
            });
        }
    }
}
