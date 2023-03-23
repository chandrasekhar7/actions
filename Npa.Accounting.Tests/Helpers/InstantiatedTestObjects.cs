

using System;
using System.Collections.Generic;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;

namespace Npa.Accounting.Tests.Helpers;
public static class TestObjects{

    public static ScheduledAch MockScheduledAch (){
        var ach = new ScheduledAch(1, TransactionType.Disburse, MockModifiedBy(), 1.1m, DateOnly.FromDateTime(DateTime.Now));

        return ach;
    }

    public static ModifiedBy MockModifiedBy () =>  new ModifiedBy(DateTime.Now, new Teller("AAA"));

    public static LoanInfo MockLoanInfo () => new LoanInfo(Location.Kansas, new Credit(1.1m, 1.1m), 1.1m, true);

    public static Loan MockLoan() => new Loan(1, 1, MockLoanInfo(),  new List<ScheduledAch>(){MockScheduledAch()});


    public static Customer MockCustomer () => new Customer(1, new CustomerInfo(), new CardStore(1, 1, new List<CustomerCard>()), MockLoan());

}