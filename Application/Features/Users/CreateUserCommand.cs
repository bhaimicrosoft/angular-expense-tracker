using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users;

public record CreateUserCommand(string Email, string FullName) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.Email, request.FullName);

        await _userRepository.AddSync(user, cancellationToken);
        return user.Id;
    }
}