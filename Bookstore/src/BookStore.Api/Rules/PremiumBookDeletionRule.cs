using BookStore.Api.Exceptions;
using BookStore.Api.Models;

namespace BookStore.Api.Rules;

public class PremiumBookDeletionRule : IBookDeletionRule
{
    public void Validate(Book book)
    {
        if (book.IsPremium)
            throw new BusinessRuleException("Premium books cannot be deleted.");
    }
}