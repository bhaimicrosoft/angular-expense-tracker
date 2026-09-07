using Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Features.Incomes;

public record UpdateIncomeCommand(
    Guid Id,
    Guid UserId,
    string Title,
    decimal Amount,
    string Currency,
    Guid CategoryId,
    DateTime IncomeDateUtc) : IRequest<bool>;

internal class UpdateIncomeCommandHandler(IIncomeRepository incomeRepository)
    : IRequestHandler<UpdateIncomeCommand, bool>
{
    public async Task<bool> Handle(UpdateIncomeCommand request, CancellationToken cancellationToken)
    {
        return await incomeRepository.UpdateAsync(request.Id, request.UserId, request.Title, request.Amount,
            request.Currency, request.CategoryId, request.IncomeDateUtc, cancellationToken);
    }
}

public class UpdateIncomeCommandValidator : AbstractValidator<UpdateIncomeCommand>
{
    public UpdateIncomeCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Income ID is required.");
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User ID is required.");
        RuleFor(v => v.Title).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Amount).GreaterThan(0);
        RuleFor(v => v.Currency).NotEmpty().Length(3);
        RuleFor(v => v.CategoryId).NotEmpty();
        RuleFor(v => v.IncomeDateUtc).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);
    }
}
