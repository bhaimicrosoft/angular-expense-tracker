using Application.Common.Pagination;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Incomes;

public record GetIncomesQuery(
    Guid UserId,
    Guid? CategoryId,
    DateTime? FromDateUtc,
    DateTime? ToDateUtc,
    int Page,
    int PageSize) : IRequest<PagedResult<Income>>;

internal class GetIncomesQueryHandler(IIncomeRepository incomeRepository)
    : IRequestHandler<GetIncomesQuery, PagedResult<Income>>
{
    public async Task<PagedResult<Income>> Handle(GetIncomesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await incomeRepository.GetByUserIdAsync(
            request.UserId,
            request.CategoryId,
            request.FromDateUtc,
            request.ToDateUtc,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<Income>(items, request.Page, request.PageSize, totalCount);
    }
}
