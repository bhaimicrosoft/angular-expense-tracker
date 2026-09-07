using API.Contracts;
using API.Extensions;
using Application.Features.Categories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/me/categories")]
public class CategoriesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(User.GetUserId(), request.Name, request.HexColor);
        var categoryId = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetCategories), null, new CreateResourceResponse(categoryId));
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(categoryId, User.GetUserId(), request.Name, request.HexColor);
        var updated = await _sender.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteCategoryCommand(categoryId, User.GetUserId()), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
  
    [HttpGet(Name = nameof(GetCategories))]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _sender.Send(new GetUserCategoriesQuery(User.GetUserId()), cancellationToken);
        return Ok(categories.Select(category => new CategoryResponse(
            category.Id,
            category.UserId,
            category.Name,
            category.HexColor)));
    }
}