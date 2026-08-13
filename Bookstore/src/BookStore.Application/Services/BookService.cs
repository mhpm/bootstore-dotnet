using BookStore.Application.Abstractions;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Mappings;

namespace BookStore.Application.Services;

public class BookService(
    IBookRepository repository,
    IEnumerable<IBookDeletionRule> deletionRules)
{
    public async Task<Result<IEnumerable<BookResponse>>> GetAllAsync()
    {
        var books = await repository.GetAllAsync();
        var response = books.Select(book => book.ToResponse()).ToList();

        return Result<IEnumerable<BookResponse>>.Success(response);
    }

    public async Task<Result<BookResponse>> GetByIdAsync(int id)
    {
        var book = await repository.GetByIdAsync(id);

        if (book is null)
        {
            return Result<BookResponse>.Failure(
                new Error("Book.NotFound", "The requested book was not found."));
        }

        return Result<BookResponse>.Success(book.ToResponse());
    }

    public async Task<Result<BookResponse>> CreateAsync(CreateBookRequest request)
    {
        var book = request.ToEntity();

        await repository.AddAsync(book);

        var createdBook = await repository.GetByIdAsync(book.Id);

        return Result<BookResponse>.Success(
            (createdBook ?? book).ToResponse());
    }

    public async Task<Result> UpdateAsync(int id, UpdateBookRequest request)
    {
        var book = await repository.GetByIdAsync(id);

        if (book is null)
        {
            return Result.Failure(
                new Error("Book.NotFound", "The requested book was not found."));
        }

        book.UpdateFrom(request);
        await repository.UpdateAsync(book);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var book = await repository.GetByIdAsync(id);

        if (book is null)
        {
            return Result.Failure(
                new Error("Book.NotFound", "The requested book was not found."));
        }

        foreach (var rule in deletionRules)
        {
            rule.Validate(book);
        }

        await repository.DeleteAsync(id);

        return Result.Success();
    }
}