using Application.Exceptions;
using Application.Interfaces;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users;

public record LoginCommand(string Email, string Password) : IRequest<string>;   


public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Find the user
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        // 2. Verify the password using Bcrypt
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new CustomValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Credentials", "Invalid email or password.")
            });
        }

        return _jwtProvider.Generate(user);
    }
}