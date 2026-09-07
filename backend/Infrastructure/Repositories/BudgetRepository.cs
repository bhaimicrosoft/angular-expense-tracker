using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BudgetRepository(AppDbContext context) : IBudgetRepository
{
    public async Task<Budget?> GetByCategoryAndDateAsync(Guid userId, Guid categoryId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await context.Budgets.AsNoTracking()
            .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId && b.Month == month && b.Year == year,
                cancellationToken);
    }

    public async Task AddAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        await context.Budgets.AddAsync(budget, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, Guid userId, Guid categoryId, decimal amount, string currency,
        int month, int year, CancellationToken cancellationToken = default)
    {
        var budget = await context.Budgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId,
            cancellationToken);
        if (budget is null)
        {
            return false;
        }

        budget.Update(categoryId, Money.Create(amount, currency), month, year);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var budget = await context.Budgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId,
            cancellationToken);
        if (budget is null)
        {
            return false;
        }

        context.Budgets.Remove(budget);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}