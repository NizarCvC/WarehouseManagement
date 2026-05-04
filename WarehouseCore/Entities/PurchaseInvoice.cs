namespace WarehouseCore.Entities;

public class PurchaseInvoice
{
    public int PurchaseInvoiceID { get; init; }
    public int InvoiceID { get; init; }
    public required Invoice Invoice { get; init; }
    public int SupplierID { get; init; }
    public required Supplier Supplier { get; init; }
}
