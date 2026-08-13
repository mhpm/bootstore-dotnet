using BookStore.Application.Abstractions;
using BookStore.Application.Exceptions;
using BookStore.Domain.Entities;

namespace BookStore.Application.Rules;

public class PremiumBookDeletionRule : IBookDeletionRule
{
    public void Validate(Book book)
    {
        if (book.IsPremium)
            throw new BusinessRuleException("Premium books cannot be deleted.");
    }
}