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

    public async Task<IEnumerable<Income>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Incomes.AsNoTracking().Where(i => i.UserId == userId).ToListAsync(cancellationToken);
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
