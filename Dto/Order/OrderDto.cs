using QLCHNT.Const;

namespace QLCHNT.Dto.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public Enums.Status Status { get; set; }
        //
        public decimal TotalPrice { get; set; }
        public List<OrderDetailDto> OrderDetails  { get; set; } = new();
    }
}
