using System;
using System.Collections.Generic;
using System.Linq;

namespace Npa.Accounting.Infrastructure.Repay;

public class RepayErrors
{
   public List<ErrorItem> Errors { get; set; }

   public override string ToString() => string.Join(Environment.NewLine, Errors.Select(e => e.ToString()));
}