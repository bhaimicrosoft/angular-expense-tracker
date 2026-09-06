using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Features.Expenses;

public record CreateExpenseCommand(
    Guid UserId,
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime ExpenseDateUtc) : IRequest<Guid>;

internal class CreateExpenseCommandHandler(IExpenseRepository expenseRepository)
    : IRequestHandler<CreateExpenseCommand, Guid>
{
    public async Task<Guid> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var moneyAmount = Money.Create(request.Amount, request.Currency);
        var expense = Expense.Create(request.UserId, request.Title, moneyAmount, request.CategoryId,
            request.ExpenseDateUtc);

        await expenseRepository.AddAsync(expense, cancellationToken);
        return expense.Id;
    }
}


public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(v => v.Amount)
            .GreaterThan(0).WithMessage("Expense amount must be strictly greater than zero.");

        RuleFor(v => v.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be exactly 3 characters long (e.g., USD, EUR, INR).");

        RuleFor(v => v.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(v => v.ExpenseDateUtc)
            .NotEmpty().WithMessage("Expense Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Expense date cannot be in the future.");
    }
}