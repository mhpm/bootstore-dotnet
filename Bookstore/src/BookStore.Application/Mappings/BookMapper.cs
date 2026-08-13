using BookStore.Application.DTOs;
using BookStore.Domain.Entities;

namespace BookStore.Application.Mappings;

public static class BookMapper
{
    public static Book ToEntity(this CreateBookRequest request)
    {
        return new Book(
            request.Title,
            request.AuthorId,
            request.ISBN,
            request.PublishedDate,
            request.Price,
            request.Stock,
            request.IsPremium,
            request.IsLoaned,
            request.IsHistorical,
            request.IsArchived);
    }

    public static BookResponse ToResponse(this Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author?.Name ?? string.Empty,
            ISBN = book.ISBN,
            PublishedDate = book.PublishedDate,
            Price = book.Price,
            Stock = book.Stock,
            IsPremium = book.IsPremium,
            IsLoaned = book.IsLoaned,
            IsHistorical = book.IsHistorical,
            IsArchived = book.IsArchived
        };
    }

    public static void UpdateFrom(this Book book, UpdateBookRequest request)
    {
        book.Update(
            request.Title,
            request.AuthorId,
            request.ISBN,
            request.PublishedDate,
            request.Price,
            request.Stock,
            request.IsPremium,
            request.IsLoaned,
            request.IsHistorical,
            request.IsArchived);
    }
}