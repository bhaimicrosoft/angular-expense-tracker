using Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
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