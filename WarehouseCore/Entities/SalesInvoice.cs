namespace WarehouseCore.Entities;

public class SalesInvoice
{
    public required int SalesInvoiceID { get; init; }
    public required int InvoiceID { get; init; }
    public Invoice Invoice { get; init; } = null!;
    public required int CustomerID { get; init; }
    public Customer Customer { get; init; } = null!;
}
