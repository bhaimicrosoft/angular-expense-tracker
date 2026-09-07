namespace API.Contracts;

public sealed record CreateIncomeRequest(
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime IncomeDateUtc);

public sealed record UpdateIncomeRequest(
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime IncomeDateUtc);

public sealed record IncomeResponse(
    Guid Id,
    Guid UserId,
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime IncomeDateUtc);
