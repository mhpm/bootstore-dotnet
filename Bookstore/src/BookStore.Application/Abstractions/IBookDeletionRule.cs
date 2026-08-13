using BookStore.Domain.Entities;

namespace BookStore.Application.Abstractions;

public interface IBookDeletionRule
{
    void Validate(Book book);
}