using System;
using System.Collections.Generic;
using System.Linq;
using Npa.Accounting.Common;

namespace Npa.Accounting.Domain.DEPRECATED.Users;

public class User
{
    public Teller Teller { get; }
    public string[] Roles { get; }
    public int? PowerId { get; }

    public User(string teller, int? powerId = null, IEnumerable<string>? roles = null) : this(new Teller(teller), powerId, roles)
    {
    }
    
    public User(Teller teller, int? powerId = null, IEnumerable<string>? roles = null)
    {
        Teller = teller;
        PowerId = powerId;
        Roles = roles?.ToArray() ?? new string[]{};
    }

    public bool IsInRole(string role) => Roles.Contains(role);
}