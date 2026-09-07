using System.ComponentModel.DataAnnotations;

namespace API.Contracts;

public sealed record CreateExpenseRequest(
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime ExpenseDateUtc);

public sealed record UpdateExpenseRequest(
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime ExpenseDateUtc);

public sealed record ExpenseResponse(
    Guid Id,
    Guid UserId,
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime ExpenseDateUtc);

public sealed class TransactionListRequest : IValidatableObject
{
    public Guid? CategoryId { get; init; }
    public DateTime? FromDateUtc { get; init; }
    public DateTime? ToDateUtc { get; init; }

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FromDateUtc.HasValue && ToDateUtc.HasValue && FromDateUtc.Value > ToDateUtc.Value)
        {
            yield return new ValidationResult(
                "FromDateUtc must be earlier than or equal to ToDateUtc.",
                [nameof(FromDateUtc), nameof(ToDateUtc)]);
        }
    }
}
