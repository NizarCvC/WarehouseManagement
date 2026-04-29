namespace WarehouseCore.DTOs.ReadDTOs;

public class CurrentStockDto
{
    public int ProductID { get; set; }
    public required string ProductName { get; set; }
    public required string WarehouseName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue { get; set; }
}
