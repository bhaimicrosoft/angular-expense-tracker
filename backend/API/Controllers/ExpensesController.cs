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
    
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetExpenses(Guid userId)
    {
        var expenses = await _sender.Send(new GetExpensesQuery(userId));

        return Ok(expenses);
    }
}