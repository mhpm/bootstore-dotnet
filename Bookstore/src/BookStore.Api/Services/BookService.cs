using BookStore.Api.Common;
using BookStore.Api.DTOs;
using BookStore.Api.Mappings;
using BookStore.Api.Repositories;
using BookStore.Api.Rules;

namespace BookStore.Api.Services;

public class BookService(IBookRepository repository, IEnumerable<IBookDeletionRule> deletionRules)
{
    public async Task<IEnumerable<BookResponse>> GetAllAsync()
    {
        return (await repository.GetAllAsync())
            .Select(book => book.ToResponse());
    }

    public async Task<BookResponse?> GetByIdAsync(int id)
    {
        var book = await repository.GetByIdAsync(id);

        return book?.ToResponse();
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        var book = request.ToEntity();

        await repository.AddAsync(book);

        var createdBook = await repository.GetByIdAsync(book.Id);

        return (createdBook ?? book).ToResponse();
    }

    public async Task<bool> UpdateAsync(int id, UpdateBookRequest request)
    {
        var book = await repository.GetByIdAsync(id);

        if (book is null)
            return false;

        book.UpdateFrom(request);

        await repository.UpdateAsync(book);

        return true;
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var book = await repository.GetByIdAsync(id);

        if (book is null)
        {
            return Result.Failure("Book not found.", StatusCodes.Status404NotFound);
        }

        foreach (var rule in deletionRules)
        {
            rule.Validate(book);
        }

        await repository.DeleteAsync(id);

        return Result.Success();
    }

}