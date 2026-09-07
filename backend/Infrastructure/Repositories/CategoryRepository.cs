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
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId,
            cancellationToken);
        if (category is null)
        {
            return false;
        }

        category.Update(name, hexColor);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId,
            cancellationToken);
        if (category is null)
        {
            return false;
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}