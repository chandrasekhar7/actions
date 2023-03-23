using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npa.Accounting.Application.CardTransactions;
using Npa.Accounting.Application.CardTransactions.Commands.AddTokenTransaction;
using Npa.Accounting.Application.CardTransactions.Queries.GetCardTransaction;
using Npa.Accounting.Application.Transactions;
using Npa.Accounting.Common;
using Npa.Accounting.Common.ErrorHandling;

namespace Npa.Accounting.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("transactions/cards")]
    public class CardTransactionsController : CqrsControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Manages card transactions
        /// </summary>
        /// <param name="mediator"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CardTransactionsController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // public DebitCardPaymentController(IMediator mediator) => this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        
        // [HttpPost, Route("cards"), MapToApiVersion("1")]
        // [ProducesResponseType(typeof(CardTransactionViewModel), StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<ActionResult<CardTransactionViewModel>> SubmitPayment(NewCardTransactionViewModel newCardTransaction)
        // {
        //     var transaction = await mediator.Send(new AddDebitCardTransactionCommand(newCardTransaction.Card, newCardTransaction));
        //     return CreatedAtRoute(nameof(TransactionsController.GetCardTransaction), new {id = transaction.Id},
        //         transaction);
        // }
        
        /// <summary>
        /// Processes a card transaction for a given loan using a tokenized card
        /// </summary>
        /// <param name="newTransaction">The transaction to process</param>
        /// <returns></returns>
        [HttpPost, Route(""), MapToApiVersion("1")]
        [ProducesResponseType(typeof(CardTransactionViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public async Task<ActionResult<CardTransactionViewModel>> SubmitTokenPayment(NewTokenTransactionViewModel newTransaction)
        {
            var transaction = await mediator.Send(new AddTokenTransactionCommand(newTransaction.Token, newTransaction, IpHelper.GetIp(HttpContext)));
            return CreatedAtRoute(nameof(GetCardTransaction), new {id = transaction.Id},transaction);
            
        }
        
        /// <summary>
        /// Gets a transaction with the given id
        /// </summary>
        /// <param name="id">Transaction id</param>
        /// <returns></returns>
        [HttpGet, Route("{id:int}", Name = "GetCardTransaction"), MapToApiVersion("1")]
        [ProducesResponseType(typeof(CardTransactionViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CardTransactionViewModel>> GetCardTransaction(int id) =>
            Ok((await mediator.Send(new GetCardTransactionQuery(new TransactionFilter() {TransactionId = id}))).FirstOrDefault() 
               ?? throw new NotFoundException());
    }
}