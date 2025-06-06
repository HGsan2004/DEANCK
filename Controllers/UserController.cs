using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLCHNT.DATA;
using QLCHNT.Dto.Category;
using QLCHNT.Dto.User;
using QLCHNT.Service.User;

namespace QLCHNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // 1 Controller tương ứng với 1 Service <=> 1 đối tượng Entity

        private readonly IUserService _userService;

        public UserController(IUserService userService) // Đăng ký DI
        {
            _userService = userService;
        }


        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>Create(UserCreateRequest request)
        {
            try
            {
                var result = await _userService.Create(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister request)
        {
            try
            {
                var token = await _userService.Register(request);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Lỗi từ hệ thống, hoặc lỗi từ người dùng,..
            }
        }


        // Đăng nhập tài khoản
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin request)
        {
            try
            {
                var token = await _userService.Login(request);
                return Ok(ApiResponse<object>.SuccessResponse(token, "Dang nhap thanh cong"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("profile")]
        [Authorize] // Bắt buộc phải xác thực 
        public async Task<IActionResult> Profile()
        {
            try
            {
                var user = await _userService.Profile();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }      
        [HttpGet("getAll")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            // Không có trả về lỗi
            // Không dùng try - catch

            var result = await _userService.GetAll();
            return Ok(result);
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var result = await _userService.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update")] // => /update
        public async Task<IActionResult> Update(UserUpdate request)
        {
            try
            {
                // hành dộng update dang bất đồng bộ => asyns và await
                var result = await _userService.Update(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                var result = await _userService.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("paging")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedOrders([FromBody] UserGetPageingRequest request)
        {
            var result = await _userService.GetPagedUsersAsync(request);
            return Ok(result);
        }
    }
}
