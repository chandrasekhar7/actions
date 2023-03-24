using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Npa.Accounting.Application.CardTransactions;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Infrastructure.Helpers;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions.Loans;

namespace Npa.Accounting.Infrastructure.Loans;

public class LoanService : ILoanService
{
    private readonly HttpClient client;
    private readonly IDbFacade dbContext;
    private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings();

    public LoanService(HttpClient client, IDbFacade dbContext)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        serializerSettings.Converters.Add(new StringEnumConverter());
    }

    public async Task<LoanInfo> GetLoan(int loanId, CancellationToken t = default)
    {
        var r = await client.GetAsync($"loans/{loanId}", t);
        await ResponseHelper.ThrowIfInvalidResponse(r);

        var data = JsonConvert.DeserializeObject<LoanResponse>(await r.Content.ReadAsStringAsync(t))
                   ?? throw new NullReferenceException(nameof(r));

        return new LoanInfo(data.Location, new Credit(data.CreditLimit, data.AvailableLimit), data.Balance.Total,
            data.LoanType != "Payday");
    }

    public async Task<LoanInfo> GetStatement(StatementId stmtId, CancellationToken t = default)
    {
        var loan = GetLoan(stmtId.LoanId, t);

        var stmtRes = client.GetAsync($"statements/{stmtId}");

        await Task.WhenAll(loan, stmtRes);
        await ResponseHelper.ThrowIfInvalidResponse(stmtRes.Result);

        var stmt = JsonConvert.DeserializeObject<StatementResponse>(await stmtRes.Result.Content.ReadAsStringAsync(t))
                   ?? throw new NullReferenceException(nameof(stmtRes.Result));
        return new LoanInfo(loan.Result.Location, new Credit(loan.Result.Credit.Limit, 0)
            , stmt.Balance.Total, loan.Result.PartialPayments,stmtId.StatementDate);
    }

    public async Task ApplyTransaction(Transaction transaction, CancellationToken t = default, int? rescindPaymentId = null)
    {
        if (transaction.TransactionType == TransactionType.Disburse)
        {
            try
            {
                // TODO move this elsewhere
                dbContext.ExecNonQuery(
                    "UPDATE C SET StatusID = 'F' FROM ILM_Clients C INNER JOIN loan.Loans L ON L.LoanID = @LoanId AND L.TransID = C.ILM_Trans_Id",
                    new { transaction.LoanId });
            }
            catch
            {
            }
        }

        var adjustmentInput = new LocalTransaction(transaction, rescindPaymentId);
        try
        {
            var result = await client.PostAsync("adjustments/apply-transaction",
                new StringContent(JsonConvert.SerializeObject(adjustmentInput, serializerSettings), Encoding.UTF8, "application/json"));
            await ResponseHelper.ThrowIfInvalidResponse(result);
        }
        catch(Exception ex)
        {
            throw new InfrastructureLayerException(ex.Message);
        }
        
    }

    public async Task ApplyCreditAsync(Loan loan, Transaction transaction, CancellationToken t = default)
    {
        throw new System.NotImplementedException();
    }

    private enum Status
    {
        Success = 1,Fail
    }

    private class LocalTransaction
    {
        public int TransactionId { get; }
        public int LoanId { get; }
        public TransactionType TransactionType { get; }
        public decimal Amount { get; }
        public DateTime CreatedOn { get; }
        public string Teller { get; }
        
        public Status Result { get; }

        public int? RescindPaymentId { get; }

        public LocalTransaction(Transaction t)
        {
            TransactionId = t.Id;
            LoanId = t.LoanId;
            TransactionType = t.TransactionType;
            Amount = Math.Abs(t.Amount);
            CreatedOn = t.CreatedOn;
            Teller = t.Teller.Value;
            Result = t.CardTransaction != null ? GetStatus(t.CardTransaction.ReturnMessage.Status) : Status.Success;
            RescindPaymentId = null;
        }
        public LocalTransaction(Transaction t, int? r = null)
        {
            TransactionId = t.Id;
            LoanId = t.LoanId;
            TransactionType = t.TransactionType;
            Amount = Math.Abs(t.Amount);
            CreatedOn = t.CreatedOn;
            Teller = t.Teller.Value;
            Result = t.CardTransaction != null ? GetStatus(t.CardTransaction.ReturnMessage.Status) : Status.Success;
            RescindPaymentId = r;
        }


        private Status GetStatus(CardReturnStatus s) => s switch
        {
            CardReturnStatus.Approve => Status.Success,
            _ => Status.Fail
        };
    }
}