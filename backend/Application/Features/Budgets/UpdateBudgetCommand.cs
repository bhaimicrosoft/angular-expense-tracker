using Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Features.Budgets;

public record UpdateBudgetCommand(Guid Id, Guid UserId, Guid CategoryId, decimal Amount, string Currency, int Month, int Year)
    : IRequest<bool>;

internal class UpdateBudgetCommandHandler(IBudgetRepository budgetRepository)
    : IRequestHandler<UpdateBudgetCommand, bool>
{
    public async Task<bool> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        return await budgetRepository.UpdateAsync(request.Id, request.UserId, request.CategoryId, request.Amount,
            request.Currency, request.Month, request.Year, cancellationToken);
    }
}

public class UpdateBudgetCommandValidator : AbstractValidator<UpdateBudgetCommand>
{
    public UpdateBudgetCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.UserId).NotEmpty();
        RuleFor(v => v.CategoryId).NotEmpty();
        RuleFor(v => v.Amount).GreaterThan(0);
        RuleFor(v => v.Currency).NotEmpty().Length(3);
        RuleFor(v => v.Month).InclusiveBetween(1, 12);
        RuleFor(v => v.Year).GreaterThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
