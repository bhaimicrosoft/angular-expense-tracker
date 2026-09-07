using Application.Features.Categories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var categoryId = await _sender.Send(command, cancellationToken);
        
        // Return 201 Created with the location of the newly created category
        return CreatedAtAction(nameof(GetCategory), new { userId = command.UserId }, new { Id = categoryId });
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (categoryId != command.Id)
        {
            return BadRequest("Category route ID must match payload ID.");
        }

        var updated = await _sender.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{categoryId:guid}/user/{userId:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, Guid userId, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteCategoryCommand(categoryId, userId), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
 
    // http:localhost/api/categories/user/{userId}
    [HttpGet("user/{userId:guid}", Name = nameof(GetCategory))]
    public async Task<IActionResult> GetCategory(Guid userId)
    {
        var categories = await _sender.Send(new GetUserCategoriesQuery(userId));
        return Ok(categories);
    }
}