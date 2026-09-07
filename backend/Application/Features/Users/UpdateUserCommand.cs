using Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Features.Users;

public record UpdateUserCommand(Guid Id, string FullName) : IRequest<bool>;

internal class UpdateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await userRepository.UpdateAsync(request.Id, request.FullName, cancellationToken);
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");
    }
}
