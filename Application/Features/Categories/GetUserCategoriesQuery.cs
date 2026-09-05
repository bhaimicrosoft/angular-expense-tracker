using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Categories;

public record GetUserCategoriesQuery(Guid userId): IRequest<IEnumerable<Category>>;


public class GetUserCategoriesQueryHandler : IRequestHandler<GetUserCategoriesQuery, IEnumerable<Category>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetUserCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<IEnumerable<Category>> Handle(GetUserCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetUserCategoriesAsync(request.userId, cancellationToken);
    }
}