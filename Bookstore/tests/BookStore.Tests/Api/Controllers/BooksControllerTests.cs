using System.Reflection;
using BookStore.Api.Controllers;
using BookStore.Application.Abstractions;
using BookStore.Application.DTOs;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookStore.Tests.Api.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly List<IBookDeletionRule> _deletionRules;
    private readonly BookService _bookService;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _deletionRules = [];
        _bookService = new BookService(_bookRepositoryMock.Object, _deletionRules);
        _controller = new BooksController(_bookService);
    }

    private static Book CreateTestBook(
        int id,
        string title = "Clean Code",
        int authorId = 1,
        string isbn = "9780132350884",
        DateOnly? publishedDate = null,
        decimal price = 35.50m,
        int stock = 10,
        bool isPremium = false,
        bool isLoaned = false,
        bool isHistorical = false,
        bool isArchived = false)
    {
        var book = new Book(
            title,
            authorId,
            isbn,
            publishedDate ?? new DateOnly(2008, 8, 1),
            price,
            stock,
            isPremium,
            isLoaned,
            isHistorical,
            isArchived);

        typeof(Book).GetProperty(nameof(Book.Id), BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(book, id);

        return book;
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WithListOfBooks_WhenBooksExist()
    {
        // Arrange
        var books = new List<Book>
        {
            CreateTestBook(1, "Clean Code", 1, "9780132350884", new DateOnly(2008, 8, 1), 35.50m, 10),
            CreateTestBook(2, "Refactoring", 2, "9780201485677", new DateOnly(1999, 7, 8), 42.00m, 5)
        };
        _bookRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(books);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<BookResponse>>().Subject;
        response.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WithEmptyList_WhenNoBooksExist()
    {
        // Arrange
        _bookRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<BookResponse>>().Subject;
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WithBook_WhenBookExists()
    {
        // Arrange
        var book = CreateTestBook(1, "Clean Architecture", 1, "9780134494166", new DateOnly(2017, 9, 20), 39.99m, 8);
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<BookResponse>().Subject;
        response.Id.Should().Be(1);
        response.Title.Should().Be("Clean Architecture");
        response.ISBN.Should().Be("9780134494166");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Book?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Domain-Driven Design",
            AuthorId = 1,
            ISBN = "9780321125217",
            PublishedDate = new DateOnly(2003, 8, 30),
            Price = 49.99m,
            Stock = 12
        };

        var createdBook = CreateTestBook(
            10,
            request.Title,
            request.AuthorId,
            request.ISBN,
            request.PublishedDate,
            request.Price,
            request.Stock);

        _bookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
            .Callback<Book>(b => typeof(Book).GetProperty(nameof(Book.Id))!.SetValue(b, 10))
            .Returns(Task.CompletedTask);
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(createdBook);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(BooksController.GetById));
        createdAtActionResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(10);
        var response = createdAtActionResult.Value.Should().BeOfType<BookResponse>().Subject;
        response.Id.Should().Be(10);
        response.Title.Should().Be("Domain-Driven Design");
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenBookIsUpdatedSuccessfully()
    {
        // Arrange
        var existingBook = CreateTestBook(5, "Legacy Code", 1, "9780131177055", new DateOnly(2004, 1, 1), 30.00m, 5);
        var updateRequest = new UpdateBookRequest
        {
            Title = "Working Effectively with Legacy Code",
            AuthorId = 1,
            ISBN = "9780131177055",
            PublishedDate = new DateOnly(2004, 1, 1),
            Price = 38.00m,
            Stock = 7
        };

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existingBook);
        _bookRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(5, updateRequest);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _bookRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Book>(b => b.Title == "Working Effectively with Legacy Code")), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenBookToUpdateDoesNotExist()
    {
        // Arrange
        var updateRequest = new UpdateBookRequest
        {
            Title = "Non Existent Book",
            AuthorId = 1,
            ISBN = "9780131177055",
            PublishedDate = new DateOnly(2004, 1, 1),
            Price = 30.00m,
            Stock = 5
        };
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Book?)null);

        // Act
        var result = await _controller.Update(99, updateRequest);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenBookIsDeletedSuccessfully()
    {
        // Arrange
        var existingBook = CreateTestBook(3, "Code Complete", 1, "9780735619678", new DateOnly(2004, 6, 9), 45.00m, 3);
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(existingBook);
        _bookRepositoryMock.Setup(r => r.DeleteAsync(3)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(3);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _bookRepositoryMock.Verify(r => r.DeleteAsync(3), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenBookToDeleteDoesNotExist()
    {
        // Arrange
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Book?)null);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
}
