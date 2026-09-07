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

    public async Task<IEnumerable<Expense>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Expenses.AsNoTracking().Where(e => e.UserId == userId).ToListAsync(cancellationToken);
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