using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npa.Accounting.Domain.DEPRECATED.Transactions
{
    public class FraudDetection
    {
        public FraudDetection(int powerID, int loanID, int cardID)
        {
            PowerID = powerID;
            LoanID = loanID;
            CardID = cardID;
        }

        public int PowerID { get; set; }
        public int LoanID { get; set; }
        public int CardID { get; set; }
    }
}
