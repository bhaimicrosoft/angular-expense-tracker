using Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Features.Categories;

public record UpdateCategoryCommand(Guid Id, Guid UserId, string Name, string HexColor) : IRequest<bool>;

internal class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<UpdateCategoryCommand, bool>
{
    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        return await categoryRepository.UpdateAsync(request.Id, request.UserId, request.Name, request.HexColor,
            cancellationToken);
    }
}

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.UserId).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(100);
        RuleFor(v => v.HexColor)
            .NotEmpty()
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
    }
}
