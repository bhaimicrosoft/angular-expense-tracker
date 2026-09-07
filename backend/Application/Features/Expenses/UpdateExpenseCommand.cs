using Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Features.Expenses;

public record UpdateExpenseCommand(
    Guid Id,
    Guid UserId,
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime ExpenseDateUtc) : IRequest<bool>;

internal class UpdateExpenseCommandHandler(IExpenseRepository expenseRepository)
    : IRequestHandler<UpdateExpenseCommand, bool>
{
    public async Task<bool> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        return await expenseRepository.UpdateAsync(request.Id, request.UserId, request.Title, request.Amount,
            request.Currency, request.CategoryId, request.ExpenseDateUtc, cancellationToken);
    }
}

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Expense ID is required.");
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User ID is required.");
        RuleFor(v => v.Title).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Amount).GreaterThan(0);
        RuleFor(v => v.Currency).NotEmpty().Length(3);
        RuleFor(v => v.CategoryId).NotEmpty();
        RuleFor(v => v.ExpenseDateUtc).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);
    }
}
