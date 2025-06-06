using QLCHNT.Dto.Category;
using QLCHNT.Dto.Order;
using QLCHNT.Dto.User;
using QLCHNT.Entity;

namespace QLCHNT.Service.User
{
    public interface IUserService 
    {
        Task<string> Register(UserRegister request);
        Task<string> Login(UserLogin request);
        Task<List<UserEntity>> GetAll();
        Task<UserEntity> Profile();

        Task<UserEntity> Get(Guid Id);

        Task<Guid> Create(UserCreateRequest request);

        Task<Guid> Update(UserUpdate request);

        Task<bool> Delete(Guid Id);
        
        Task<PagedResult<UserDto>> GetPagedUsersAsync(UserGetPageingRequest request);
    }
}
