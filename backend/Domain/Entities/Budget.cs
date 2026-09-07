using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Budget
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }

    public Money Limit { get; private set; } = null!;
    public int Month { get; private set; }
    public int Year { get; private set; }

    private Budget()
    {
    }

    public static Budget Create(Guid userId, Guid categoryId, Money limit, int month, int year)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        return new Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = categoryId,
            Limit = limit,
            Month = month,
            Year = year
        };
    }

    public void Update(Guid categoryId, Money limit, int month, int year)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        CategoryId = categoryId;
        Limit = limit;
        Month = month;
        Year = year;
    }
}