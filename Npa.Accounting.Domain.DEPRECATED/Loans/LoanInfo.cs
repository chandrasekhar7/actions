using System;
using Npa.Accounting.Domain.DEPRECATED.Customers;

namespace Npa.Accounting.Domain.DEPRECATED.Loans;

public record LoanInfo(Location Location, Credit Credit, decimal Balance, bool PartialPayments, DateOnly? StatementId = null);