namespace WarehouseCore.Entities;

public class SalesInvoice
{
    public int SalesInvoiceID { get; init; }
    public int InvoiceID { get; init; }
    public required Invoice Invoice { get; init; }
    public int CustomerID { get; init; }
    public required Customer Customer { get; init; }
}
