using Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;


    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await _sender.Send(command);
        
        // Return 201 Created with the location of the newly created user
        return CreatedAtAction(nameof(GetUser), new { userId = userId }, new {Id = userId});
    }

    [HttpGet("{userId:guid}", Name = nameof(GetUser))]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var user = await _sender.Send(new GetUserByIdQuery(userId));
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}