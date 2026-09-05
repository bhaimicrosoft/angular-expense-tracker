using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Budgets;

public abstract record GetBudgetQuery(Guid CategoryId, int Month, int Year) : IRequest<Budget?>;

public class GetBudgetQueryHandler(IBudgetRepository budgetRepo) : IRequestHandler<GetBudgetQuery, Budget?>
{
    public async Task<Budget?> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
    {
        return await budgetRepo.GetByCategoryAndDateAsync(request.CategoryId, request.Month, request.Year,
            cancellationToken);
    }
}