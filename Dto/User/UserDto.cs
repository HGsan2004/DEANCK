using QLCHNT.Const;

namespace QLCHNT.Dto.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public Enums.Role Role { get; set; }
    }
}
