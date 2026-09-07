using API.Contracts;
using API.Extensions;
using Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/me")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(User.GetUserId(), request.FullName);
        var updated = await sender.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var deleted = await sender.Send(new DeleteUserCommand(userId), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet(Name = nameof(GetCurrentUser))]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByIdQuery(User.GetUserId()), cancellationToken);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserResponse(user.Id, user.Email, user.FullName));
    }
}