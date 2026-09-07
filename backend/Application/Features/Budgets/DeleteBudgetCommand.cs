using Domain.Repositories;
using MediatR;

namespace Application.Features.Budgets;

public record DeleteBudgetCommand(Guid Id, Guid UserId) : IRequest<bool>;

internal class DeleteBudgetCommandHandler(IBudgetRepository budgetRepository)
    : IRequestHandler<DeleteBudgetCommand, bool>
{
    public async Task<bool> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        return await budgetRepository.DeleteAsync(request.Id, request.UserId, cancellationToken);
    }
}
