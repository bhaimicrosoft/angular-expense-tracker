using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Incomes;

public record GetIncomesQuery(Guid UserId) : IRequest<IEnumerable<Income>>;

internal class GetIncomesQueryHandler(IIncomeRepository incomeRepository)
    : IRequestHandler<GetIncomesQuery, IEnumerable<Income>>
{
    public async Task<IEnumerable<Income>> Handle(GetIncomesQuery request, CancellationToken cancellationToken)
    {
        return await incomeRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
    }
}
