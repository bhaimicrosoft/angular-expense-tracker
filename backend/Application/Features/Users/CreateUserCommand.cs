using Domain.Entities;
using Domain.Repositories;
using FluentValidation;
using MediatR;


namespace Application.Features.Users;

public record CreateUserCommand(string Email, string FullName, string Password) : IRequest<Guid>;

internal class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Hash the password before creating the User
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 11);

        // Create user with the new password
        var user = User.Create(request.Email, request.FullName, passwordHash);

        await userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");
        
        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
    }
}