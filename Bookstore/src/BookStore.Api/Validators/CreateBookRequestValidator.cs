using BookStore.Api.DTOs;
using FluentValidation;

namespace BookStore.Api.Validators;

public class CreateBookRequestValidator
    : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AuthorId)
            .GreaterThan(0);

        RuleFor(x => x.ISBN)
            .NotEmpty()
            .Length(13);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PublishedDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
    }
}