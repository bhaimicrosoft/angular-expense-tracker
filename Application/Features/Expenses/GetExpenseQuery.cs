using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Expenses;

public abstract record GetExpensesQuery(Guid UserId) : IRequest<IEnumerable<Expense>>;

public class GetExpensesQueryHandler(IExpenseRepository expenseRepository)
    : IRequestHandler<GetExpensesQuery, IEnumerable<Expense>>
{
    public async Task<IEnumerable<Expense>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        return await expenseRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
    }
}