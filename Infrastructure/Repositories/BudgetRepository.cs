using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BudgetRepository(AppDbContext context) : IBudgetRepository
{
    public async Task<Budget?> GetByCategoryAndDateAsync(Guid categoryId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await context.Budgets.AsNoTracking()
            .FirstOrDefaultAsync(b => b.CategoryId == categoryId && b.Month == month && b.Year == year,
                cancellationToken);
    }

    public async Task AddAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        await context.Budgets.AddAsync(budget, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}