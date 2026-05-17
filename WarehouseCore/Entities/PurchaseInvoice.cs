namespace WarehouseCore.Entities;

public class PurchaseInvoice
{
    public int PurchaseInvoiceID { get; init; }
    public int InvoiceID { get; init; }
    public Invoice Invoice { get; init; } = null!;
    public int SupplierID { get; init; }
    public Supplier Supplier { get; init; } = null!;
}
