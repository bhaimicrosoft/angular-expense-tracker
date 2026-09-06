using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtProvider
{
    string Generate(User user);
}