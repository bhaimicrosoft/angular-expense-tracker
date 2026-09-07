using API.Contracts;
using API.Extensions;
using Application.Features.Incomes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/me/incomes")]
public class IncomesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateIncome([FromBody] CreateIncomeRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateIncomeCommand(
            User.GetUserId(),
            request.Title,
            request.Amount,
            request.Currency,
            request.CategoryId,
            request.IncomeDateUtc);

        var incomeId = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetIncomes), null, new CreateResourceResponse(incomeId));
    }

    [HttpPut("{incomeId:guid}")]
    public async Task<IActionResult> UpdateIncome(Guid incomeId, [FromBody] UpdateIncomeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateIncomeCommand(
            incomeId,
            User.GetUserId(),
            request.Title,
            request.Amount,
            request.Currency,
            request.CategoryId,
            request.IncomeDateUtc);

        var updated = await _sender.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{incomeId:guid}")]
    public async Task<IActionResult> DeleteIncome(Guid incomeId, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteIncomeCommand(incomeId, User.GetUserId()), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetIncomes([FromQuery] TransactionListRequest request, CancellationToken cancellationToken)
    {
        var incomes = await _sender.Send(new GetIncomesQuery(
            User.GetUserId(),
            request.CategoryId,
            request.FromDateUtc,
            request.ToDateUtc,
            request.Page,
            request.PageSize), cancellationToken);

        return Ok(new PagedResponse<IncomeResponse>(
            incomes.Items.Select(income => new IncomeResponse(
                income.Id,
                income.UserId,
                income.Title,
                income.Amount.Amount,
                income.Amount.Currency,
                income.CategoryId,
                income.IncomeDateUtc)).ToArray(),
            incomes.Page,
            incomes.PageSize,
            incomes.TotalCount,
            incomes.TotalPages));
    }
}
