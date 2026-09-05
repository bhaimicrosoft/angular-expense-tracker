using Domain.Entities;
using Domain.Repositories;
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
}