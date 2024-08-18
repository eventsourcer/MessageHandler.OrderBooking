using Azure.Search.Documents.Indexes;
using OrderBooking.Projections;

public class SalesOrder
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = string.Empty;

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string Name { get; set; } = string.Empty;

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public string? Status { get; set; }
    
    [SimpleField(IsSortable = true)]
    public int Amount { get; set; }
}