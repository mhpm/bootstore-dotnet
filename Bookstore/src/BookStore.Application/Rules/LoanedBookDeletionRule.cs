using BookStore.Application.Abstractions;
using BookStore.Domain.Entities;

namespace BookStore.Application.Rules;

public class LoanedBookDeletionRule : IBookDeletionRule
{
    public void Validate(Book book)
    {
        if (book.IsLoaned)
            throw new InvalidOperationException("The book is currently loaned and cannot be deleted.");
    }
}