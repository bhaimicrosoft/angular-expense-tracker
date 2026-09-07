using API.Contracts;
using API.Extensions;
using Application.Features.Budgets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/me/budgets")]
public class BudgetsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateBudgetCommand(
            User.GetUserId(),
            request.CategoryId,
            request.Amount,
            request.Currency,
            request.Month,
            request.Year);

        var budgetId = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetBudget),
            new { categoryId = request.CategoryId, year = request.Year, month = request.Month },
            new CreateResourceResponse(budgetId));
    }

    [HttpPut("{budgetId:guid}")]
    public async Task<IActionResult> UpdateBudget(Guid budgetId, [FromBody] UpdateBudgetRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBudgetCommand(
            budgetId,
            User.GetUserId(),
            request.CategoryId,
            request.Amount,
            request.Currency,
            request.Month,
            request.Year);

        var updated = await _sender.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{budgetId:guid}")]
    public async Task<IActionResult> DeleteBudget(Guid budgetId, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteBudgetCommand(budgetId, User.GetUserId()), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{categoryId:guid}/{year:int}/{month:int}")]
    public async Task<IActionResult> GetBudget(
        [FromRoute] Guid categoryId,
        [FromRoute] int year,
        [FromRoute] int month,
        CancellationToken cancellationToken)
    {
        var budget = await _sender.Send(new GetBudgetQuery(User.GetUserId(), categoryId, month, year), cancellationToken);

        if (budget is null)
        {
            return NotFound();
        }

        return Ok(new BudgetResponse(
            budget.Id,
            budget.UserId,
            budget.CategoryId,
            budget.Limit.Amount,
            budget.Limit.Currency,
            budget.Month,
            budget.Year));
    }
}