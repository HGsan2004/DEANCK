using QLCHNT.Const;

namespace QLCHNT.Entity
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone {  get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Enums.Role Role { get; set; }
        //public vir//tual List<CartEntity> Carts { get; set; }
        //public virtual List<OrderEntity> Orders { get; set; }
    }
}
