using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using QLCHNT.Dto.Category;
using QLCHNT.Dto.User;
using QLCHNT.Entity;
using QLCHNT.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QLCHNT.Dto.Order;

namespace QLCHNT.Service.User
{
    public class UserService : IUserService
    {
        private readonly IRepository<UserEntity> _rpUserRepository; // Gọi thẳng hành động trên bảng Users
        private readonly IMapper _mapper; // mapper từ DTO sang Entity, ....
        private readonly IPasswordHasher<UserEntity> _passwordHasher; // Băm mật khẩu của người dùng
        private readonly IHttpContextAccessor _httpContextAccessor; // Lấy thông tin người dùng từ token
        private readonly IConfiguration _configuaration; // Lấy thông tin trong appsettings.json

        public UserService(
            IRepository<UserEntity> rpUser,
            IMapper mapper,
            IPasswordHasher<UserEntity> passwordHasher,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration
            )
        {
            _rpUserRepository = rpUser;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _configuaration = configuration;
        }
        public async Task<Guid> Create(UserCreateRequest request)
        {
            var userExist = await _rpUserRepository.FirstOrDefault(u => u.Email == request.Email);
            if (userExist != null)
            {
                throw new Exception("Email đã tồn tại");
            }

            var entity = _mapper.Map<UserEntity>(request);
            // thiếu mật khẩu, => tự băm mật khẩu và tự lưu vào entity
            entity.Password = _passwordHasher.HashPassword(entity, request.Password);

            var result = await _rpUserRepository.CreateAsync(entity);

            return result.Id;
        }    

        public async Task<UserEntity> Get(Guid Id)
        {
            var userExit = await _rpUserRepository.GetAsync(Id);
            return userExit;
        }

        public async Task<List<UserEntity>> GetAll()
        {
            var users = await _rpUserRepository.GetAllAsync();
            return users;
        }

        public async Task<string> Login(UserLogin request)
        {
            var user = await _rpUserRepository.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            // So sánh mật khẩu người dùng với mật khẩu trong CSDL
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            // Có trạng thái là Failed, nếu mật khẩu không đúng
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Mâtk khẩu không đúng");
            }

            return GenerateToken(user);
        }

        public async Task<string> Register(UserRegister request)
        {
           
            var userExist = await _rpUserRepository.FirstOrDefault(u => u.Email == request.Email);
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Mật khẩu không được để trống");
            }
            //Mapper tuwf UserRegisterRequest sang UserEntity (đã đinh nghĩa mapper chưa???)

            var entity = _mapper.Map<UserEntity>(request);
            // thiếu mật khẩu, => tự băm mật khẩu và tự lưu vào entity
            entity.Password = _passwordHasher.HashPassword(entity, request.Password);

            var result = await _rpUserRepository.CreateAsync(entity);

            return GenerateToken(result);

        }

        public async Task<bool> Delete(Guid id)
        {
            var user = await _rpUserRepository.GetAsync(id); // ✅ Đợi xong trước
            if (user == null)
                throw new Exception("Người dùng không tồn tại");

            await _rpUserRepository.DeleteAsync(id); // ✅ Gọi tiếp sau khi xong
            return true;
        }

        public async Task<PagedResult<UserDto>> GetPagedUsersAsync(UserGetPageingRequest request)
        {
            var query = _rpUserRepository.AsQueryable();

            // 🔍 Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                request.SearchText = request.SearchText.ToLower();
                // use contains to search: id, phone, address, productName
                query = query.Where(o => o.Phone.ToString().ToLower().Contains(request.SearchText)
                                         || o.Username.ToLower().Contains(request.SearchText)
                                         || o.Address.ToLower().Contains(request.SearchText)
                                         || o.Phone.ToString().ToLower().Contains(request.SearchText));
            }
            
            // Tổng và phân trang
            var total = query.Count();
            if (request.PageIndex.HasValue && request.PageSize.HasValue)
            {
                query = query.Skip((request.PageIndex.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
            }

            var mappedItems = _mapper.Map<List<UserDto>>(query.ToList());

            return new PagedResult<UserDto>
            {
                TotalItems = total,
                Items = mappedItems
            };
        }

        public async Task<Guid> Update(UserUpdate request)
        {
            
            var userupdateExist = await _rpUserRepository.GetAsync(request.Id);
            if (userupdateExist.Username == request.Username && userupdateExist.Email == request.Email && userupdateExist.Phone == request.Phone )
            {
                throw new Exception("Thông tin đã tồn tại");
            }
                
            // Mapper sang UserEntity
            _mapper.Map(request, userupdateExist);

            await _rpUserRepository.UpdateAsync(userupdateExist);

            // Cập nhaatj thanhf coong
            return userupdateExist.Id;
        }
        public async Task<UserEntity> Profile()
        {
            // Chỉ cần có token ở header
            // lấy thôgn tin từ token => decode JWT => cho dữ liệu
            // HTTContextAccessor: lấy thông tin từ token
            // String => Guid: Chuyển từ string sang Guid dùng Guid.Parse
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.First(u => u.Type == "Id").Value);

            //Tìm thông tin user theo Id đã lấy từ token
            var user = await _rpUserRepository.GetAsync(userId); // Tái sử dụng Repo: Generic Repository

            // UserEntity => Mapper

            return _mapper.Map<UserEntity>(user);
        }
        // Tạo token cho người dùng => Trả về token dạng string
        private string GenerateToken(UserEntity user)
        {
            var jwtSettings = _configuaration.GetSection("Jwt");


            var claims = new[]
            {
                // Không hiện thị, hệ thống Authorize sẽ chỉ đọc có ClaimType.Role
                new Claim(ClaimTypes.Role, user.Role.ToString()),  // Không public
                new Claim("Role", user.Role.ToString()),
                new Claim("Name", user.Username),
                new Claim("Email", user.Email),
                new Claim("Id", user.Id.ToString()), // Lấy Id Claim. Type == Id => String => Guid
            };

            var key = new Microsoft.IdentityModel.Tokens
                .SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));
            var creds = new Microsoft.IdentityModel.Tokens
                .SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
       
    }   
}
