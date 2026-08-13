using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookStore.Infrastructure.Data;

public sealed class BookStoreDbContextFactory : IDesignTimeDbContextFactory<BookStoreDbContext>
{
    public BookStoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BookStoreDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("BOOKSTORE_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=BookStore;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new BookStoreDbContext(optionsBuilder.Options);
    }
}
