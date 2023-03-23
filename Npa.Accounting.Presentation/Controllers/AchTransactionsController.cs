using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npa.Accounting.Application.AchTransactions;
using Npa.Accounting.Application.AchTransactions.Queries;
using Npa.Accounting.Application.Transactions;

namespace Npa.Accounting.Presentation.Controllers;

[ApiController]
[ApiVersion("1")]
[Authorize]
[Route("transactions/ach")]
public class AchTransactionsController : ControllerBase
{
    private readonly IMediator mediator;

    public AchTransactionsController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet, Route(""), MapToApiVersion("1")]
    [ProducesResponseType(typeof(IEnumerable<AchTransactionViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AchTransactionViewModel>>> GetAchTransactionHistory(
        [FromQuery] int? limit,
        [FromQuery] int? loanId, [FromQuery] int? powerId, [FromQuery] bool? pending)
    {
        // TODO: Streamline this permission check
        var pid = ((ClaimsIdentity) User.Identity).Claims
            .Where(c => c.Type.ToUpper() == "POWERID")
            .Select(c => c.Value)
            .FirstOrDefault();
        int? userId = null;
        if (int.TryParse(pid, out int i))
        {
            userId = i;
        }


        if (powerId == null)
        {
            powerId = userId;
        }

        if (!User.IsInRole("administrator") && powerId != userId)
        {
            return Forbid();
        }

        var filter = new TransactionFilter()
        {
            PowerId = powerId,
            LoanId = loanId,
            Pending = pending
        };
        return Ok(await mediator.Send(new GetAchTransactionsQuery(filter)));
    }
}

//TODO: validate that the loan being taken action against is owned by the user in the token