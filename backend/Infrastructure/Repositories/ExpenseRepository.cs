using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ExpenseRepository(AppDbContext context): IExpenseRepository
{
    public async Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Expenses.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Expense> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        Guid? categoryId = null,
        DateTime? fromDateUtc = null,
        DateTime? toDateUtc = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = context.Expenses.AsNoTracking().Where(e => e.UserId == userId);

        if (categoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == categoryId.Value);
        }

        if (fromDateUtc.HasValue)
        {
            query = query.Where(e => e.ExpenseDateUtc >= fromDateUtc.Value);
        }

        if (toDateUtc.HasValue)
        {
            query = query.Where(e => e.ExpenseDateUtc <= toDateUtc.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(e => e.ExpenseDateUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await context.Expenses.AddAsync(expense, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, Guid userId, string title, decimal amount, string currency,
        Guid categoryId, DateTime expenseDateUtc, CancellationToken cancellationToken = default)
    {
        var expense = await context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId,
            cancellationToken);
        if (expense is null)
        {
            return false;
        }

        expense.Update(title, Money.Create(amount, currency), categoryId, expenseDateUtc);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var expense = await context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId,
            cancellationToken);
        if (expense is null)
        {
            return false;
        }

        context.Expenses.Remove(expense);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}