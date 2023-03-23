using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npa.Accounting.Application.AchTransactions.Queries;
using Npa.Accounting.Application.CardTransactions.Queries.GetCardTransaction;
using Npa.Accounting.Application.Transactions;

namespace Npa.Accounting.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator mediator;

        public TransactionsController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets the transaction history for the specified Loan Id or Power Id. Scope can be limited with RecordLimit.
        /// </summary>
        /// <param name="limit">Number of records to limit the response to</param>
        /// <param name="loanId">The LoanId for the loan to retrieve transaction history on</param>
        /// <param name="powerId">The PowerId for the customer to retrieve transaction history on</param>
        /// <returns>IEnumerable/<TransactionsModel/></returns>
        [HttpGet, Route(""), MapToApiVersion("1")]
        [ProducesResponseType(typeof(TransactionsViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionsViewModel>>> GetTransactionHistory([FromQuery]int? limit, [FromQuery] int? loanId, [FromQuery] int? powerId)
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
                LoanId = loanId
            };

            return Ok(new TransactionsViewModel()
            {
                CardTransactions = await mediator.Send(new GetCardTransactionQuery(filter)),
                AchTransactions = await mediator.Send(new GetAchTransactionsQuery(filter)),
            });
        }
    }
}