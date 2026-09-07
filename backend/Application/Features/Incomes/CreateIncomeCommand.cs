using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Features.Incomes;

public record CreateIncomeCommand(
    Guid UserId,
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime IncomeDateUtc) : IRequest<Guid>;

internal class CreateIncomeCommandHandler(IIncomeRepository incomeRepository)
    : IRequestHandler<CreateIncomeCommand, Guid>
{
    public async Task<Guid> Handle(CreateIncomeCommand request, CancellationToken cancellationToken)
    {
        var moneyAmount = Money.Create(request.Amount, request.Currency);
        var income = Income.Create(request.UserId, request.Title, moneyAmount, request.CategoryId,
            request.IncomeDateUtc);

        await incomeRepository.AddAsync(income, cancellationToken);
        return income.Id;
    }
}

public class CreateIncomeCommandValidator : AbstractValidator<CreateIncomeCommand>
{
    public CreateIncomeCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(v => v.Amount)
            .GreaterThan(0).WithMessage("Income amount must be strictly greater than zero.");

        RuleFor(v => v.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be exactly 3 characters long (e.g., USD, EUR, INR).");

        RuleFor(v => v.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(v => v.IncomeDateUtc)
            .NotEmpty().WithMessage("Income Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Income date cannot be in the future.");
    }
}
