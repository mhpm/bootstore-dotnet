namespace BookStore.Domain.Entities;

public class Book
{
    public int Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public int AuthorId { get; private set; }

    public Author? Author { get; private set; }

    public string ISBN { get; private set; } = string.Empty;

    public DateOnly PublishedDate { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public bool IsPremium { get; private set; }

    public bool IsLoaned { get; private set; }

    public bool IsHistorical { get; private set; }

    public bool IsArchived { get; private set; }

    private Book() { }

    public Book(
        string title,
        int authorId,
        string isbn,
        DateOnly publishedDate,
        decimal price,
        int stock,
        bool isPremium,
        bool isLoaned,
        bool isHistorical,
        bool isArchived)
    {
        Title = title;
        AuthorId = authorId;
        ISBN = isbn;
        PublishedDate = publishedDate;
        Price = price;
        Stock = stock;
        IsPremium = isPremium;
        IsLoaned = isLoaned;
        IsHistorical = isHistorical;
        IsArchived = isArchived;
    }

    public void Update(
        string title,
        int authorId,
        string isbn,
        DateOnly publishedDate,
        decimal price,
        int stock,
        bool isPremium,
        bool isLoaned,
        bool isHistorical,
        bool isArchived)
    {
        Title = title;
        AuthorId = authorId;
        ISBN = isbn;
        PublishedDate = publishedDate;
        Price = price;
        Stock = stock;
        IsPremium = isPremium;
        IsLoaned = isLoaned;
        IsHistorical = isHistorical;
        IsArchived = isArchived;
    }
}