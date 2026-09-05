using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users;

public record GetUserByIdQuery(Guid UserId) : IRequest<User?>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
    }
}