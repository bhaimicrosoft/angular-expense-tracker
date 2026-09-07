namespace Domain.Entities;

public sealed class Category
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } // to track category belongs to which userId

    public string Name { get; private set; } = string.Empty;
    public string HexColor { get; private set; } = string.Empty;

    private Category()
    {
    }

    public static Category Create(Guid userId, string name, string hexColor)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name.Trim(),
            HexColor = hexColor.Trim()
        };
    }

    public void Update(string name, string hexColor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(hexColor);

        Name = name.Trim();
        HexColor = hexColor.Trim();
    }
}