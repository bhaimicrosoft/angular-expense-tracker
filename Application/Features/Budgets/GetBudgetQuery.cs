using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Budgets;

public record GetBudgetQuery(Guid categoryId, int Month, int Year) : IRequest<Budget?>;

public class GetBudgetQueryHandler : IRequestHandler<GetBudgetQuery, Budget?>
{
    private readonly IBudgetRepository _budgetRepo;

    public GetBudgetQueryHandler(IBudgetRepository budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public async Task<Budget?> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
    {
        return await _budgetRepo.GetByCategoryAndDateAsync(request.categoryId, request.Month, request.Year,
            cancellationToken);
    }
}