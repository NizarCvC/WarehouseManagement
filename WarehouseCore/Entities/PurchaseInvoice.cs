namespace WarehouseCore.Entities;

public class PurchaseInvoice
{
    public required int PurchaseInvoiceID { get; init; }
    public required int InvoiceID { get; init; }
    public Invoice Invoice { get; init; } = null!;
    public required int SupplierID { get; init; }
    public Supplier Supplier { get; init; } = null!;
}
