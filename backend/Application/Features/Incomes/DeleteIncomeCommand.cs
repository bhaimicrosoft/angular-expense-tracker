using Domain.Repositories;
using MediatR;

namespace Application.Features.Incomes;

public record DeleteIncomeCommand(Guid Id, Guid UserId) : IRequest<bool>;

internal class DeleteIncomeCommandHandler(IIncomeRepository incomeRepository)
    : IRequestHandler<DeleteIncomeCommand, bool>
{
    public async Task<bool> Handle(DeleteIncomeCommand request, CancellationToken cancellationToken)
    {
        return await incomeRepository.DeleteAsync(request.Id, request.UserId, cancellationToken);
    }
}
