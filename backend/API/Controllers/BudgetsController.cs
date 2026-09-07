using Application.Features.Budgets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetCommand command)
    {
        var budgetId = await _sender.Send(command);

        return Ok(new { Id = budgetId });
    }

    [HttpPut("{budgetId:guid}")]
    public async Task<IActionResult> UpdateBudget(Guid budgetId, [FromBody] UpdateBudgetCommand command)
    {
        if (budgetId != command.Id)
        {
            return BadRequest("Budget route ID must match payload ID.");
        }

        var updated = await _sender.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{budgetId:guid}/user/{userId:guid}")]
    public async Task<IActionResult> DeleteBudget(Guid budgetId, Guid userId)
    {
        var deleted = await _sender.Send(new DeleteBudgetCommand(budgetId, userId));
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{categoryId:guid}/{year:int}/{month:int}")]
    public async Task<IActionResult> GetBudget([FromRoute] Guid categoryId, [FromRoute] int year, [FromRoute] int month)
    {
        var budget = await _sender.Send(new GetBudgetQuery(categoryId, month, year));

        if (budget is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            budget.Id,
            budget.UserId,
            budget.CategoryId,
            Amount = budget.Limit.Amount,
            Currency = budget.Limit.Currency,
            budget.Month,
            budget.Year
        });
    }
}