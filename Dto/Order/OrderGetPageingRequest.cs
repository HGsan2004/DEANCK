using QLCHNT.Const;

namespace QLCHNT.Dto.Order;

public class OrderGetPageingRequest : PagedRequestDto
{
    public Guid? UserId { get; set; }
    public Enums.Status? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    //Product: name
    //User: username, email, phone, address = search
}