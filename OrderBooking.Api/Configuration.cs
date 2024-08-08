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
}