using API.Contracts;
using API.Extensions;
using Application.Features.Expenses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/me/expenses")]
public class ExpensesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateExpenseCommand(
            User.GetUserId(),
            request.Title,
            request.Amount,
            request.Currency,
            request.CategoryId,
            request.ExpenseDateUtc);

        var expenseId = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetExpenses), null, new CreateResourceResponse(expenseId));
    }

    [HttpPut("{expenseId:guid}")]
    public async Task<IActionResult> UpdateExpense(Guid expenseId, [FromBody] UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateExpenseCommand(
            expenseId,
            User.GetUserId(),
            request.Title,
            request.Amount,
            request.Currency,
            request.CategoryId,
            request.ExpenseDateUtc);

        var updated = await _sender.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{expenseId:guid}")]
    public async Task<IActionResult> DeleteExpense(Guid expenseId, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteExpenseCommand(expenseId, User.GetUserId()), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
      
    [HttpGet]
    public async Task<IActionResult> GetExpenses([FromQuery] TransactionListRequest request, CancellationToken cancellationToken)
    {
        var expenses = await _sender.Send(new GetExpensesQuery(
            User.GetUserId(),
            request.CategoryId,
            request.FromDateUtc,
            request.ToDateUtc,
            request.Page,
            request.PageSize), cancellationToken);

        return Ok(new PagedResponse<ExpenseResponse>(
            expenses.Items.Select(expense => new ExpenseResponse(
                expense.Id,
                expense.UserId,
                expense.Title,
                expense.Amount.Amount,
                expense.Amount.Currency,
                expense.CategoryId,
                expense.ExpenseDateUtc)).ToArray(),
            expenses.Page,
            expenses.PageSize,
            expenses.TotalCount,
            expenses.TotalPages));
    }
}