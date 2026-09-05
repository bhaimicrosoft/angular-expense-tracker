using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Budgets;

// Payload (Command)
public abstract record CreateBudgetCommand(Guid UserId, Guid CategoryId, decimal Amount, string Currency, int Month, int Year) : IRequest<Guid>;



// Behavior
public class CreateBudgetCommandHandler(IBudgetRepository budgetRepo) : IRequestHandler<CreateBudgetCommand, Guid>
{
    public async Task<Guid> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var limit = Money.Create(request.Amount, request.Currency);
        var budget = Budget.Create(request.UserId, request.CategoryId, limit, request.Month, request.Year);

        await budgetRepo.AddAsync(budget, cancellationToken);
        return budget.Id;
    }
}