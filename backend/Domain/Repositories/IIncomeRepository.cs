using Domain.Entities;

namespace Domain.Repositories;

public interface IIncomeRepository
{
    Task<Income?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Income>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Income income, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, Guid userId, string title, decimal amount, string currency, Guid categoryId,
        DateTime incomeDateUtc, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
