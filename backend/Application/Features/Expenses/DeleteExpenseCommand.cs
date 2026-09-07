using Domain.Repositories;
using MediatR;

namespace Application.Features.Expenses;

public record DeleteExpenseCommand(Guid Id, Guid UserId) : IRequest<bool>;

internal class DeleteExpenseCommandHandler(IExpenseRepository expenseRepository)
    : IRequestHandler<DeleteExpenseCommand, bool>
{
    public async Task<bool> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        return await expenseRepository.DeleteAsync(request.Id, request.UserId, cancellationToken);
    }
}
