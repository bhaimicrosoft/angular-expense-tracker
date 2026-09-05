using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Expenses;

public record GetExpensesQuery(Guid UserId) : IRequest<IEnumerable<Expense>>;

public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, IEnumerable<Expense>>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpensesQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<IEnumerable<Expense>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        return await _expenseRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
    }
}