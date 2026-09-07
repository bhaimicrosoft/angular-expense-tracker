using Application.Features.Incomes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncomesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateIncome([FromBody] CreateIncomeCommand command)
    {
        var incomeId = await _sender.Send(command);

        return Ok(new { Id = incomeId });
    }

    [HttpPut("{incomeId:guid}")]
    public async Task<IActionResult> UpdateIncome(Guid incomeId, [FromBody] UpdateIncomeCommand command)
    {
        if (incomeId != command.Id)
        {
            return BadRequest("Income route ID must match payload ID.");
        }

        var updated = await _sender.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{incomeId:guid}/user/{userId:guid}")]
    public async Task<IActionResult> DeleteIncome(Guid incomeId, Guid userId)
    {
        var deleted = await _sender.Send(new DeleteIncomeCommand(incomeId, userId));
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetIncomes(Guid userId)
    {
        var incomes = await _sender.Send(new GetIncomesQuery(userId));

        return Ok(incomes.Select(income => new
        {
            income.Id,
            income.UserId,
            income.Title,
            Amount = income.Amount.Amount,
            Currency = income.Amount.Currency,
            income.CategoryId,
            income.IncomeDateUtc
        }));
    }
}
