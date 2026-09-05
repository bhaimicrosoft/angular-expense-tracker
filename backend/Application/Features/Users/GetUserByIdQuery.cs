using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users;

public record GetUserByIdQuery(Guid UserId) : IRequest<User?>;

public class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, User?>
{
    public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await userRepository.GetByIdAsync(request.UserId, cancellationToken);
    }
}