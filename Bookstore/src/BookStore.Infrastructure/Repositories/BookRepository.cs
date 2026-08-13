using BookStore.Application.Abstractions;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

public class BookRepository(BookStoreDbContext context) : IBookRepository
{
    private readonly BookStoreDbContext _context = context;

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .AsNoTracking()
            .Include(book => book.Author)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
            .Include(book => book.Author)
            .FirstOrDefaultAsync(book => book.Id == id);
    }

    public async Task AddAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        var existingBook = await _context.Books.FindAsync(book.Id);

        if (existingBook is null)
            return;

        existingBook.Update(
            book.Title,
            book.AuthorId,
            book.ISBN,
            book.PublishedDate,
            book.Price,
            book.Stock,
            book.IsPremium,
            book.IsLoaned,
            book.IsHistorical,
            book.IsArchived);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book is null)
            return;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}