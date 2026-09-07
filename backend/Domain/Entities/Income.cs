using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Income
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public DateTime IncomeDateUtc { get; private set; }

    private Income()
    {
    }

    public static Income Create(Guid userId, string title, Money amount, Guid categoryId, DateTime incomeDateUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new Income
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title.Trim(),
            Amount = amount,
            CategoryId = categoryId,
            IncomeDateUtc = incomeDateUtc
        };
    }

    public void Update(string title, Money amount, Guid categoryId, DateTime incomeDateUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Title = title.Trim();
        Amount = amount;
        CategoryId = categoryId;
        IncomeDateUtc = incomeDateUtc;
    }
}
