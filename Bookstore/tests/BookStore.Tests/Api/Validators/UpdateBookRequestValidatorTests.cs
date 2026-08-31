using BookStore.Api.Validators;
using BookStore.Application.DTOs;
using FluentValidation.TestHelper;

namespace BookStore.Tests.Api.Validators;

public class UpdateBookRequestValidatorTests
{
    private readonly UpdateBookRequestValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_TitleIsEmpty()
    {
        // Arrange
        var request = new UpdateBookRequest
        {
            Title = "",
            AuthorId = 1,
            ISBN = "9780132350884",
            PublishedDate = DateOnly.FromDateTime(DateTime.Today),
            Price = 29.99m,
            Stock = 10
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }

    [Fact]
    public void Should_HaveError_When_TitleExceeds200Characters()
    {
        // Arrange
        var longTitle = new string('B', 201);
        var request = new UpdateBookRequest
        {
            Title = longTitle,
            AuthorId = 1,
            ISBN = "9780132350884",
            PublishedDate = DateOnly.FromDateTime(DateTime.Today),
            Price = 29.99m,
            Stock = 10
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot exceed 200 characters.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_When_AuthorIdIsZeroOrNegative(int authorId)
    {
        // Arrange
        var request = new UpdateBookRequest
        {
            Title = "Valid Title",
            AuthorId = authorId,
            ISBN = "9780132350884",
            PublishedDate = DateOnly.FromDateTime(DateTime.Today),
            Price = 29.99m,
            Stock = 10
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AuthorId)
            .WithErrorMessage("AuthorId must be greater than 0.");
    }

    [Fact]
    public void Should_HaveError_When_ISBNIsEmpty()
    {
        // Arrange
        var request = new UpdateBookRequest
        {
            Title = "Valid Title",
            AuthorId = 1,
            ISBN = "",
            PublishedDate = DateOnly.FromDateTime(DateTime.Today),
            Price = 29.99m,
            Stock = 10
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ISBN)
            .WithErrorMessage("ISBN is required.");
    }

    [Theory]
    [InlineData("123456789012")] // 12 chars
    [InlineData("12345678901234")] // 14 chars
    public void Should_HaveError_When_ISBNIsNot13Characters(string isbn)
    {
        // Arrange
        var request = new UpdateBookRequest
        {
            Title = "Valid Title",
            AuthorId = 1,
            ISBN = isbn,
            PublishedDate = DateOnly.FromDateTime(DateTime.Today),
            Price = 29.99m,
            Stock = 10
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ISBN)
            .WithErrorMessage("ISBN must contain exactly 13 characters.");
    }

    [Fact]
    public void Should_HaveError_When_PublishedDateIsInFuture()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var request = new UpdateBookRequest
        {
            Title = "Valid Title",
            AuthorId = 1,
            ISBN = "9780132350884",
            PublishedDate = futureDate,
            Price = 29.99m,
            Stock = 10
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PublishedDate)
            .WithErrorMessage("PublishedDate cannot be in the future.");
    }

    [Fact]
    public void Should_HaveError_When_PriceIsNegative()
    {
        // Arrange
        var request = new UpdateBookRequest
        {
            Title = "Valid Title",
            AuthorId = 1,
            ISBN = "9780132350884",
            PublishedDate = DateOnly.FromDateTime(DateTime.Today),
            Price = -1.00m,
            Stock = 10
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price must be greater than or equal to 0.");
    }

    [Fact]
    public void Should_HaveError_When_StockIsNegative()
    {
        // Arrange
        var request = new UpdateBookRequest
        {
            Title = "Valid Title",
            AuthorId = 1,
            ISBN = "9780132350884",
            PublishedDate = DateOnly.FromDateTime(DateTime.Today),
            Price = 29.99m,
            Stock = -1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock)
            .WithErrorMessage("Stock must be greater than or equal to 0.");
    }

    [Fact]
    public void Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        var request = new UpdateBookRequest
        {
            Title = "Clean Architecture",
            AuthorId = 1,
            ISBN = "9780134494166",
            PublishedDate = new DateOnly(2017, 9, 20),
            Price = 39.99m,
            Stock = 8
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
