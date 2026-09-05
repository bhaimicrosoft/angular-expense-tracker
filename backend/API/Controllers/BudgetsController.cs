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

    [HttpGet("{categoryId:guid}/{year:int}/{month:int}")]
    public async Task<IActionResult> GetBudget(Guid categoryId, int year, int month)
    {
        var budget = await _sender.Send(new GetBudgetQuery(categoryId, year, month));

        return budget is not null ? Ok(budget) : NotFound();
    }
}