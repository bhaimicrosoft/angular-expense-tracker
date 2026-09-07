using Domain.Entities;

namespace Domain.Repositories;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Expense> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        Guid? categoryId = null,
        DateTime? fromDateUtc = null,
        DateTime? toDateUtc = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, Guid userId, string title, decimal amount, string currency, Guid categoryId,
        DateTime expenseDateUtc, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
