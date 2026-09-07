using Domain.Entities;

namespace Domain.Repositories;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Expense>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, Guid userId, string title, decimal amount, string currency, Guid categoryId,
        DateTime expenseDateUtc, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
