using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Expenses;

public record CreateExpenseCommand(
    Guid UserId,
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime ExpenseDateUtc) : IRequest<Guid>;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Guid>
{
    private readonly IExpenseRepository _expenseRepository;

    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<Guid> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var moneyAmount = Money.Create(request.Amount, request.Currency);
        var expense = Expense.Create(request.UserId, request.Title, moneyAmount, request.CategoryId,
            request.ExpenseDateUtc);

        await _expenseRepository.AddAsync(expense, cancellationToken);
        return expense.Id;
    }
}