namespace QLCHNT.Dto.Order;

public class RevenueResultDto
{
    public decimal TotalRevenue { get; set; }

    public List<CategorySalesDto> CategorySales { get; set; } = new();
    public int NewUsers { get; set; }
}
public class CategorySalesDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
}