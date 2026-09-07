namespace API.Contracts;

public sealed record CreateCategoryRequest(string Name, string HexColor);

public sealed record UpdateCategoryRequest(string Name, string HexColor);

public sealed record CategoryResponse(Guid Id, Guid UserId, string Name, string HexColor);
