namespace API.Contracts;

public sealed record CreateBudgetRequest(Guid CategoryId, decimal Amount, string Currency, int Month, int Year);

public sealed record UpdateBudgetRequest(Guid CategoryId, decimal Amount, string Currency, int Month, int Year);

public sealed record BudgetResponse(
    Guid Id,
    Guid UserId,
    Guid CategoryId,
    decimal Amount,
    string Currency,
    int Month,
    int Year);
