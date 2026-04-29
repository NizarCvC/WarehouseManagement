namespace WarehouseCore.DTOs.ReadDTOs;

public class ItemLedgerDto
{
    public int InventoryTransactionID { get; set; }
    public DateTime TransactionDate { get; set; }
    public required string ProductName { get; set; }
    public required string WarehouseName { get; set; }
    public required string TransactionType { get; set; } 
    public required string MovementType { get; set; }   
    public decimal Quantity { get; set; }
    public string? ReferenceDocument { get; set; }      
    public required string CreatedByName { get; set; }   
}