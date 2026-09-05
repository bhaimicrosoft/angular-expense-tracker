namespace Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;

    private User()
    {
    }

    public static User Create(string email, string fullName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim(),
            FullName = fullName.Trim()
        };
    }
}
