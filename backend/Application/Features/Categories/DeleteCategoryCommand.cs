using Domain.Repositories;
using MediatR;

namespace Application.Features.Categories;

public record DeleteCategoryCommand(Guid Id, Guid UserId) : IRequest<bool>;

internal class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        return await categoryRepository.DeleteAsync(request.Id, request.UserId, cancellationToken);
    }
}
