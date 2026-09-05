using Domain.Entities;

namespace Domain.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetUserCategoriesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);
}