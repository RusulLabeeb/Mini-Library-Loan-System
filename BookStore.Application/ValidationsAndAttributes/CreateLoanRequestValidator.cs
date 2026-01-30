using BookStore.Application.DTOs;
using FluentValidation;

namespace BookStore.Application.ValidationsAndAttributes;

public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(l => l.BookId)
            .GreaterThan(0)
            .WithMessage("BookId must be greater than 0");
    }
}
