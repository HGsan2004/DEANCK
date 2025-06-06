using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLCHNT.Const;
using QLCHNT.Dto.Order;
using QLCHNT.Service.Order;

namespace QLCHNT.Controllers;
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(OrderCreateRequest request)
        {
            try
            {
                var result = await _orderService.CreateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderService.GetAllAsync();
            return Ok(result);
        }

        
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _orderService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
        
        [HttpGet("user{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUser([FromQuery] Guid userId)
        {
            var result = await _orderService.GetByUserAsync(userId);
            return Ok(result);
        }
        
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] Enums.Status status)
        {
            var result = await _orderService.UpdateStatusAsync(id, status);
            return result == null ? NotFound() : Ok(result);
        }
        
        
        [HttpPost("revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenueStatistic([FromBody] RevenueRequestDto request)
        {
            var result = await _orderService.GetRevenueStatisticAsync(request);
            return Ok(result);
        }
        
        [HttpPost("paging")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedOrders([FromBody] OrderGetPageingRequest request)
        {
            var result = await _orderService.GetPagedOrdersAsync(request);
            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _orderService.DeleteAsync(id);
            return success ? Ok() : BadRequest("Không thể huỷ đơn.");
        }
}