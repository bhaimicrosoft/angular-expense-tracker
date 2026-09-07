using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Budgets;

public record GetBudgetQuery(Guid UserId, Guid CategoryId, int Month, int Year) : IRequest<Budget?>;

internal class GetBudgetQueryHandler(IBudgetRepository budgetRepo) : IRequestHandler<GetBudgetQuery, Budget?>
{
    public async Task<Budget?> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
    {
        return await budgetRepo.GetByCategoryAndDateAsync(request.UserId, request.CategoryId, request.Month, request.Year,
            cancellationToken);
    }
}