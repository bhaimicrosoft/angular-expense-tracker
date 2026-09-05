using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Budgets;

// Payload (Command)
public record CreateBudgetCommand(Guid userId, Guid categoryId, decimal Amount, string Currency, int Month, int Year): IRequest<Guid>;



// Behavior
public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, Guid>
{
    private readonly IBudgetRepository _budgetRepo;

    public CreateBudgetCommandHandler(IBudgetRepository budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }
    
    public async Task<Guid> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var limit = Money.Create(request.Amount, request.Currency);
        var budget = Budget.Create(request.userId, request.categoryId, limit, request.Month, request.Year);

        await _budgetRepo.AddAsync(budget, cancellationToken);
        return budget.Id;
    }
}