using BookStore.Application.Abstractions;
using BookStore.Domain.Entities;

namespace BookStore.Application.Rules;

public class HistoricalBookDeletionRule : IBookDeletionRule
{
    public void Validate(Book book)
    {
        if (book.IsHistorical && !book.IsArchived)
            throw new InvalidOperationException("Historical books must be archived before they can be deleted.");
    }
}