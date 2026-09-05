using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Expense
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = null!; // null suppression operator
    public Guid CategoryId { get; private set; }
    public DateTime ExpenseDateUtc { get; private set; } // UTC is the standard timezone

    private Expense()
    {
    }

    public static Expense Create(Guid userId, string title, Money amount, Guid categoryId, DateTime expenseDateUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title.Trim(),
            Amount = amount,
            CategoryId = categoryId,
            ExpenseDateUtc = expenseDateUtc
        };
    }
}