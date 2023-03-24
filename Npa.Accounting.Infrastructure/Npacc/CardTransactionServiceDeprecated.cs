using System;
using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Cards;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Infrastructure.Abstractions;

namespace Npa.Accounting.Infrastructure.Npacc
{
    internal class CardTransactionServiceDeprecated : ICardTransactionServiceDeprecated
    {
        private readonly IRepayService repay;
        private readonly ILppService lpp;

        public CardTransactionServiceDeprecated(IRepayService repay, ILppService lpp)
        {
            this.repay = repay ?? throw new ArgumentNullException(nameof(repay));
            this.lpp = lpp ?? throw new ArgumentNullException(nameof(lpp));
        }
        private static CardReturnStatus GetCode(string s) => s switch
        {
            "A" => CardReturnStatus.Approve,
            "D" => CardReturnStatus.Deny,
            "E" => CardReturnStatus.Error,
            "N" => CardReturnStatus.NotStarted,
            _ => throw new InvalidOperationException($"{s} is not a valid code. Valid codes are A,D,E,N")
        };

        public async Task<ReturnMessage> Process(Transaction ct, CancellationToken t = default)
        {
            if (ct.CardTransaction != null)
            {
                if (ct.CardTransaction.MerchantId.IsRepay() && ct.TransactionType == TransactionType.Debit)
                {
                    var result = await repay.DebitAsync(new NpaTokenPayment(ct.CardTransaction.Card.CardToken, ct.CardTransaction.MerchantId,
                        ct.CardTransaction.Card.PowerId,
                        ct.Amount), t);

                    return new ReturnMessage(GetCode(result.ReturnCode), result.ReturnCode, result.ReturnMessage ?? String.Empty, result.RefNum);
                }

                if (ct.CardTransaction.MerchantId.IsLPP() && ct.TransactionType == TransactionType.Debit)
                {
                    return await lpp.DebitAsync(ct, t);
                }

                if (ct.CardTransaction.MerchantId.IsLPP() && ct.TransactionType == TransactionType.Disburse)
                {
                    return await lpp.DisburseAsync(ct, t);
                }
            }
            
            throw new InfrastructureLayerException(
                $"Valid {ct.TransactionType.ToString()} processor for merchant {ct.CardTransaction?.MerchantId.ToString()} not found");
        }
    }
}