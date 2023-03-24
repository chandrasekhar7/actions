using System;

namespace Npa.Accounting.Infrastructure.Authentication;

public class AuthBearerToken
{
    public string Owner { get; set; }
    public string[] Roles { get; set; }
    public string Name { get; set; } = "FLX";
    public string Key { get; set; }
    public AuthBearerToken() { }
    public AuthBearerToken(string key)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }
}