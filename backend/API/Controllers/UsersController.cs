using Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (userId != command.Id)
        {
            return BadRequest("User route ID must match payload ID.");
        }

        var updated = await sender.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        var deleted = await sender.Send(new DeleteUserCommand(userId), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{userId:guid}", Name = nameof(GetUser))]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var user = await sender.Send(new GetUserByIdQuery(userId));
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName
        });
    }
}