using Domain.Entities;

namespace Domain.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByCategoryAndDateAsync(Guid userId, Guid categoryId, int month, int year,
        CancellationToken cancellationToken = default);

    Task AddAsync(Budget budget, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, Guid userId, Guid categoryId, decimal amount, string currency, int month, int year,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}