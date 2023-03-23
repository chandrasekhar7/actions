using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Npa.Accounting.Common;

public static class IpHelper
{
    public static string GetIp(HttpContext context)
    {
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault("").Split(',')[0];
        return string.IsNullOrEmpty(forwarded) ? context.Connection.RemoteIpAddress?.ToString() : forwarded;
    }
}