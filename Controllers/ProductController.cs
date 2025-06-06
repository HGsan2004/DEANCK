using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLCHNT.Dto.Category;
using QLCHNT.Dto.Product;
using QLCHNT.Dto.User;
using QLCHNT.Service.Product;

namespace QLCHNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;


        public ProductController(IProductService service)
        {
            _service = service;
        }


        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductCreateRequest request)
        {
            try
            {
                var result = await _service.Create(request); // Guid
                return Ok(result); // Status 200
            }
            catch (Exception ex) // bắt lỗi
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            // Không có trả về lỗi
            // Không dùng try - catch

            var result = await _service.GetAll();
            return Ok(result);
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var result = await _service.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update")] // => /update
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(ProductUpdate request)
        {
            try
            {
                // hành dộng update dang bất đồng bộ => asyns và await
                var result = await _service.Update(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                var result = await _service.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("paging")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedOrders([FromBody] ProductGetPageingRequest request)
        {
            var result = await _service.GetPagedProductsAsync(request);
            return Ok(result);
        }
        
        [HttpPost("upload-image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] Guid productId)
        {
            try
            {
                var url = await _service.UploadProductImageAsync(file, productId);
                return Ok(new { imageUrl = url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
