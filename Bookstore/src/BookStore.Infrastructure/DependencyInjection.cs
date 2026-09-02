using BookStore.Infrastructure.Data;
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
        services.AddDbContext<BookStoreDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("BookStore"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName));
        });

        return services;
    }
}
