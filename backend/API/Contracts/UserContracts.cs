namespace API.Contracts;

public sealed record UpdateCurrentUserRequest(string? Email, string FullName);

public sealed record UserResponse(Guid Id, string Email, string FullName);
