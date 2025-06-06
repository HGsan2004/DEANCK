using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLCHNT.Dto.Category;
using QLCHNT.Service.Category;

namespace QLCHNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Category : ControllerBase
    {
        private readonly ICategoryServices _services;

        public Category(ICategoryServices services)
        {
            _services = services;
        }

        
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>Create(CategoryCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _services.Create(request);
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
            var result = await _services.GetAll();
            return Ok(result);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>Get(Guid id)
        {
            try
            {
                var result = await _services.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        
        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(CategoryUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _services.Update(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _services.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
