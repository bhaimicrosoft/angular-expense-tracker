using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Categories;

public record CreateCategoryCommand(Guid userId, string Name, string HexColor): IRequest<Guid>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.userId, request.Name, request.HexColor);

        await _categoryRepository.AddAsync(category, cancellationToken);
        return category.Id;
    }
}