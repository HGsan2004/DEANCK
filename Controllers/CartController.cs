using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLCHNT.Dto.Cart;
using QLCHNT.Service.Cart;
using System.Security.Claims;

namespace QLCHNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddCartItemDto dto)
        {
            try
            {
                var result = await _cartService.AddToCartAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            // Không có trả về lỗi
            // Không dùng try - catch

            var result = await _cartService.GetAll();
            return Ok(result);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var cart = await _cartService.GetByUserIdAsync(userId);
            return Ok(cart);
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateCartItem request)
        {
            try
            {
                var result = await _cartService.UpdateCartItemAsync(request);

                return Ok(new
                {
                    message = "Cập nhật giỏ hàng thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Cập nhật giỏ hàng thất bại",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                var result = await _cartService.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
