using System;
using System.Collections.Generic;
using System.Linq;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using CardTransaction = Npa.Accounting.Domain.DEPRECATED.Transactions.CardTransaction;

namespace Npa.Accounting.Domain.DEPRECATED.Customers
{
    public class Customer : Entity<int>
    {
        public CustomerInfo CustomerInfo { get; }
        public CardStore? CardStore { get; private set; }
        public Loan Loan { get; }

        private Customer()
        {
        }

        public Customer(int id,CustomerInfo customerInfo, CardStore? cardStore, Loan loan)
        {
            Id = id;
            CardStore = cardStore;
            CustomerInfo = customerInfo;
            Loan = loan;
        }

        public void CreateCardStore(int btid, IEnumerable<CustomerCard> cards)
        {
            CardStore = new CardStore(btid, Id, cards.ToList());
        }
        
        public Transaction DisburseCard(int cardToken, decimal amount, Teller teller)
        {
            var card = CardStore?.Cards.FirstOrDefault(c => c.CardToken == cardToken) ??
                       throw new NullReferenceException();
            ThrowIfInvalidDisburse(amount);
            ThrowIfInvalidCardDisburse(card);
            
            Loan.AddTransaction(TransactionType.Disburse, amount, teller, card);
            return Loan.Transactions.Last();
        }
        
        public ScheduledAch ScheduleDisburse(decimal amount, Teller teller)
        {
            ThrowIfInvalidDisburse(amount);

            Loan.AddScheduledAch(TransactionType.Disburse, amount, teller);
            return Loan.ScheduledAch.Last();
        }

        public Transaction Debit(int cardToken, decimal amount, Teller teller)
        {
            var card = CardStore?.Cards.FirstOrDefault(c => c.CardToken == cardToken) ?? throw new NullReferenceException();
            Loan.AddTransaction(TransactionType.Debit, amount, teller, card);
            return Loan.Transactions.Last();
        }

        private void ThrowIfInvalidDisburse(decimal amount)
        {
            if (!CustomerInfo.CanFund())
            {
                throw new DomainLayerException("Customer cannot request additional funds");
            }

            if (amount < 50)
            {
                throw new DomainLayerException("Minimum draw is 50");
            }
        }

        private void ThrowIfInvalidCardDisburse(CustomerCard card)
        {
            if (!(card.CanDisburse ?? true))
            {
                throw new DomainLayerException("Customers card cannot be disbursed to");
            }
        }
    }
}