using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Categories;

public abstract record GetUserCategoriesQuery(Guid UserId): IRequest<IEnumerable<Category>>;


public class GetUserCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetUserCategoriesQuery, IEnumerable<Category>>
{
    public async Task<IEnumerable<Category>> Handle(GetUserCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await categoryRepository.GetUserCategoriesAsync(request.UserId, cancellationToken);
    }
}