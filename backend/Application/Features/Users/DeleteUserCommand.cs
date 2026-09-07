using Domain.Repositories;
using MediatR;

namespace Application.Features.Users;

public record DeleteUserCommand(Guid Id) : IRequest<bool>;

internal class DeleteUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await userRepository.DeleteAsync(request.Id, cancellationToken);
    }
}
