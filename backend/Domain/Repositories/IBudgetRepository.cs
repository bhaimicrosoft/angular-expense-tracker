using Domain.Entities;

namespace Domain.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByCategoryAndDateAsync(Guid categoryId, int month, int year,
        CancellationToken cancellationToken = default);

    Task AddAsync(Budget budget, CancellationToken cancellationToken = default);
}