using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<Data.BookStoreDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("BookStore"));
        });

        return services;
    }
}
