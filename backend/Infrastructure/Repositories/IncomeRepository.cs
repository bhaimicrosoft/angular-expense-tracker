using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class IncomeRepository(AppDbContext context) : IIncomeRepository
{
    public async Task<Income?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Incomes.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Income> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        Guid? categoryId = null,
        DateTime? fromDateUtc = null,
        DateTime? toDateUtc = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = context.Incomes.AsNoTracking().Where(i => i.UserId == userId);

        if (categoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == categoryId.Value);
        }

        if (fromDateUtc.HasValue)
        {
            query = query.Where(i => i.IncomeDateUtc >= fromDateUtc.Value);
        }

        if (toDateUtc.HasValue)
        {
            query = query.Where(i => i.IncomeDateUtc <= toDateUtc.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(i => i.IncomeDateUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Income income, CancellationToken cancellationToken = default)
    {
        await context.Incomes.AddAsync(income, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, Guid userId, string title, decimal amount, string currency,
        Guid categoryId, DateTime incomeDateUtc, CancellationToken cancellationToken = default)
    {
        var income = await context.Incomes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId,
            cancellationToken);
        if (income is null)
        {
            return false;
        }

        income.Update(title, Money.Create(amount, currency), categoryId, incomeDateUtc);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var income = await context.Incomes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId,
            cancellationToken);
        if (income is null)
        {
            return false;
        }

        context.Incomes.Remove(income);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
