using System;
using Npa.Accounting.Common;

namespace Npa.Accounting.Domain.DEPRECATED.Scheduled;

public record ModifiedBy(DateTime TimeStamp, Teller Teller);