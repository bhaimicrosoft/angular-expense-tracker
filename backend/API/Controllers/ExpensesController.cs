using Application.Features.Expenses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseCommand command)
    {
        var expenseId = await _sender.Send(command);

        return Ok(new { Id = expenseId });
    }

    [HttpPut("{expenseId:guid}")]
    public async Task<IActionResult> UpdateExpense(Guid expenseId, [FromBody] UpdateExpenseCommand command)
    {
        if (expenseId != command.Id)
        {
            return BadRequest("Expense route ID must match payload ID.");
        }

        var updated = await _sender.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{expenseId:guid}/user/{userId:guid}")]
    public async Task<IActionResult> DeleteExpense(Guid expenseId, Guid userId)
    {
        var deleted = await _sender.Send(new DeleteExpenseCommand(expenseId, userId));
        return deleted ? NoContent() : NotFound();
    }
     
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetExpenses(Guid userId)
    {
        var expenses = await _sender.Send(new GetExpensesQuery(userId));

        return Ok(expenses.Select(expense => new
        {
            expense.Id,
            expense.UserId,
            expense.Title,
            Amount = expense.Amount.Amount,
            Currency = expense.Amount.Currency,
            expense.CategoryId,
            expense.ExpenseDateUtc
        }));
    }
}