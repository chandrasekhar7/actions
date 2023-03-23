using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npa.Accounting.Application.Scheduled;
using Npa.Accounting.Application.Scheduled.Commands;
using Npa.Accounting.Application.Scheduled.Queries;
using Npa.Accounting.Application.Transactions;
using Npa.Accounting.Common;
using Npa.Accounting.Infrastructure.Authentication;

namespace Npa.Accounting.Presentation.Controllers;

[ApiController]
[ApiVersion("1")]
[Authorize]
[Route("scheduled")]
public class ScheduleTransactionController : ControllerBase
{
    private readonly IMediator mediator;
    public ScheduleTransactionController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Schedules an ach draw for the specified loanId
    /// </summary>
    /// <param name="filter">Contains the loanId and dollar amount for scheduling an ach transaction</param>
    /// <returns>IEnumerable/<TransactionsModel/></returns>
    [HttpPost, Route("ach"), MapToApiVersion("1")]
    [ProducesResponseType(typeof(ScheduledAchViewModel), StatusCodes.Status201Created)]
    public async Task<ActionResult<ScheduledAchViewModel>> ScheduleAch([FromBody] CreateScheduledAchViewModel a)
    {
        var ach = await mediator.Send(new ScheduleAchCommand(a.LoanId, a.Amount, a.TransactionType, IpHelper.GetIp(HttpContext)));
        return CreatedAtRoute(nameof(GetScheduledAch), new {id = ach.ScheduleId}, ach);
    }

    /// <summary>
    /// Gets scheduled ach by loan Id
    /// </summary>

    /// <returns>IEnumerable/<TransactionsModel/></returns>
    [HttpGet, Route("ach", Name = "GetScheduledAchByLoanId"), MapToApiVersion("1")]
    [ProducesResponseType(typeof(List<ScheduledAchViewModel>), StatusCodes.Status200OK)]
    public Task<List<ScheduledAchViewModel>> GetScheduledAchByLoanId([FromQuery]int loanId) => mediator.Send(new GetScheduledAchByLoanIdQuery(loanId));

    /// <summary>
    /// Gets scheduled ach by scheduled Id
    /// </summary>

    /// <returns>IEnumerable/<TransactionsModel/></returns>
    [HttpGet, Route("ach/{id:int}", Name = "GetScheduledAch"), MapToApiVersion("1")]
    [ProducesResponseType(typeof(ScheduledAchViewModel), StatusCodes.Status200OK)]
    public Task<ScheduledAchViewModel> GetScheduledAch(int id) => mediator.Send(new ScheduleAchQuery(id));


    /// <summary>
    /// Cancels a scheduled ach draw for the specified loanId
    /// </summary>
    /// <param name="achId">Contains the id of the scheduled ach to cancel</param>
    /// <returns>StatusCodeResult/<NoContentResult/></returns>
    [HttpDelete, Route("ach/{achId:int}", Name = "CancelScheduledAch"), MapToApiVersion("1")]
    [Authorize(Policy = Policies.Admin)]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    public async Task<NoContentResult> CancelScheduledAch([FromRoute] int achId)
    {
        await mediator.Send(new CancelScheduledAchCommand(achId));
        return NoContent();
    }
}