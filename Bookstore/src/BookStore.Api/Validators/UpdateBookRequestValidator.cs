using BookStore.Api.DTOs;
using FluentValidation;

namespace BookStore.Api.Validators;

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0)
            .WithMessage("AuthorId must be greater than 0.");

        RuleFor(x => x.ISBN)
            .NotEmpty()
            .WithMessage("ISBN is required.")
            .Length(13)
            .WithMessage("ISBN must contain exactly 13 characters.");

        RuleFor(x => x.PublishedDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("PublishedDate cannot be in the future.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock must be greater than or equal to 0.");
    }
}