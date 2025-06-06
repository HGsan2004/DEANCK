namespace QLCHNT.Dto.Order;

public class PagedRequestDto
{
    public int? PageIndex { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
    //

}