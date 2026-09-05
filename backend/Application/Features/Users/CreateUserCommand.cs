using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users;

public record CreateUserCommand(string Email, string FullName) : IRequest<Guid>;

public class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.Email, request.FullName);

        await userRepository.AddSync(user, cancellationToken);
        return user.Id;
    }
}