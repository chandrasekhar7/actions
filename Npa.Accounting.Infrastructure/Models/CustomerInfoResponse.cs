using System;

namespace Npa.Accounting.Infrastructure.Models
{
    public class CustomerInfoResponse
    {
        public int PowerId { get; set; }
        public int CreditLimit { get; set; }
        public bool PraLimit { get; set; }
        public bool ManagementBlock { get; set; }
        public int Location { get; set; }
        public MilitaryStatus? MilitaryStatus { get; set; }
    }

    public class MilitaryStatus
    {
        public bool IsMilitary { get; set; }
        public DateTime DateTime { get; set; }
        public string Path { get; set; }
    }
}
