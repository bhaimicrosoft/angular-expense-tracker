using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Categories;

public record CreateCategoryCommand(Guid UserId, string Name, string HexColor) : IRequest<Guid>;

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.UserId, request.Name, request.HexColor);

        await categoryRepository.AddAsync(category, cancellationToken);
        return category.Id;
    }
}