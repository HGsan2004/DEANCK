namespace QLCHNT.Dto.User
{
    public class UserRegister
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Phone {  get; set; }
        public string? Address { get; set; }
    }
}
