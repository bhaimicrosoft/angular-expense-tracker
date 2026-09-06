using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Features.Budgets;

// Payload (Command)
public record CreateBudgetCommand(Guid UserId, Guid CategoryId, decimal Amount, string Currency, int Month, int Year) : IRequest<Guid>;



// Behavior
internal class CreateBudgetCommandHandler(IBudgetRepository budgetRepo) : IRequestHandler<CreateBudgetCommand, Guid>
{
    public async Task<Guid> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var limit = Money.Create(request.Amount, request.Currency);
        var budget = Budget.Create(request.UserId, request.CategoryId, limit, request.Month, request.Year);

        await budgetRepo.AddAsync(budget, cancellationToken);
        return budget.Id;
    }
}

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(v => v.CategoryId).NotEmpty().WithMessage("CategoryId is required.");
        
        RuleFor(v => v.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        
        RuleFor(v => v.Month).InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");
        
        RuleFor(v => v.Year).GreaterThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Year must be equal to the current year. Past or future years are not allowed");
    }
}