using Domain.Entities;
using Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Features.Categories;

public record CreateCategoryCommand(Guid UserId, string Name, string HexColor) : IRequest<Guid>;

internal class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.UserId, request.Name, request.HexColor);

        await categoryRepository.AddAsync(category, cancellationToken);
        return category.Id;
    }
}


public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("UserId is required.");
        
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Category Name is required.")
            .MaximumLength(100).WithMessage("Category Name must not exceed 100 characters.");
        
        RuleFor(v => v.HexColor)
            .NotEmpty().WithMessage("HexColor is required.")
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$").WithMessage("HexColor must be a valid hex color code. #FFF or #FFFFFF");
    }
}