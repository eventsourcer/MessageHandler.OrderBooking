using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;

public static class SearchConfiguration
{
    public static IServiceCollection AddAzureSearch(this IServiceCollection services, string endpoint, string apiKey)
    {
        var searchClient = new SearchClient(new Uri(endpoint), "salesorders", new AzureKeyCredential(apiKey));
        var indexClient = new SearchIndexClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

        services.AddSingleton(searchClient);
        services.AddSingleton(indexClient);

        services.AddHostedService<AutoCreateIndex>();

        return services;
    }
}