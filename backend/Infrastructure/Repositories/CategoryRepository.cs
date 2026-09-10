using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetUserCategoriesAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await context.Categories.AsNoTracking().Where(c => c.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, Guid userId, string name, string hexColor,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeGuid(id);
        var normalizedUserId = NormalizeGuid(userId);
        var trimmedName = name.Trim();
        var trimmedHexColor = hexColor.Trim();

        var rowsAffected = await context.Database.ExecuteSqlInterpolatedAsync(
            $@"UPDATE Categories
               SET Name = {trimmedName}, HexColor = {trimmedHexColor}
               WHERE lower(Id) = {normalizedId} AND lower(UserId) = {normalizedUserId}",
            cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeGuid(id);
        var normalizedUserId = NormalizeGuid(userId);

        var rowsAffected = await context.Database.ExecuteSqlInterpolatedAsync(
            $@"DELETE FROM Categories
               WHERE lower(Id) = {normalizedId} AND lower(UserId) = {normalizedUserId}",
            cancellationToken);

        return rowsAffected > 0;
    }

    private static string NormalizeGuid(Guid value) => value.ToString("D").ToLowerInvariant();
}
