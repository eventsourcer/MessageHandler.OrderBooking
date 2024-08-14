using Azure.Search.Documents.Indexes;

namespace OrderBooking.Api.Models;

public class SalesOrder
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = string.Empty;

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string Name { get; set; } = string.Empty;

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public string Status { get; set; } = string.Empty;
    
    [SimpleField(IsSortable = true)]
    public int Amount { get; set; }
}