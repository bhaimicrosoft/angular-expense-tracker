using Application.Common.Pagination;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Expenses;

public record GetExpensesQuery(
    Guid UserId,
    Guid? CategoryId,
    DateTime? FromDateUtc,
    DateTime? ToDateUtc,
    int Page,
    int PageSize) : IRequest<PagedResult<Expense>>;

internal class GetExpensesQueryHandler(IExpenseRepository expenseRepository)
    : IRequestHandler<GetExpensesQuery, PagedResult<Expense>>
{
    public async Task<PagedResult<Expense>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await expenseRepository.GetByUserIdAsync(
            request.UserId,
            request.CategoryId,
            request.FromDateUtc,
            request.ToDateUtc,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<Expense>(items, request.Page, request.PageSize, totalCount);
    }
}