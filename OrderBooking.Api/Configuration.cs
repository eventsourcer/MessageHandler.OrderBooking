using Azure;
using Azure.Search.Documents;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace OrderBooking.Api;

public static class Configuration
{
    public static IServiceCollection AddCorsConfguration(
        this IServiceCollection services,
        Action<CorsOptions> conf)
        {
            services.AddCors(options => conf(options));
            return services;
        }
    public static IServiceCollection AddSearchConfiguration(this IServiceCollection services, string endpoint, string apiKey)
    {
        var searchClient = new SearchClient(new Uri(endpoint), "salesorders", new AzureKeyCredential(apiKey));

        services.AddSingleton(searchClient);

        return services;
    }

}