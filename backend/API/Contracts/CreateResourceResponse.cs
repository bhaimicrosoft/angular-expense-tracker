namespace API.Contracts;

public sealed record CreateResourceResponse(Guid Id);

public sealed record PagedResponse<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
