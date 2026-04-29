namespace WarehouseCore.DTOs.ReadDTOs;

public class InvoiceSummaryDto
{
    public int InvoiceID { get; set; }
    public required string InvoiceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string InvoiceType { get; set; } 
    public required string PartyName { get; set; }   
    public required string WarehouseName { get; set; }
    public decimal NetTotal { get; set; }           
    public required string StatusName { get; set; }  
}
