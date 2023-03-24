using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Users;
using Npa.Accounting.Domain.DEPRECATED.Users;

namespace Npa.Accounting.Infrastructure.Users;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _context;

    public UserService(IHttpContextAccessor context)
    {
        _context = context;
    }

    public User GetUser()
    {
        var identity = (ClaimsIdentity) _context.HttpContext.User?.Identity;
        var pid = int.Parse(identity.Claims.FirstOrDefault(f => f.Type.ToLower() == "powerid")?.Value ?? "0");
        
        var teller = identity.Claims.FirstOrDefault(f => f.Type.ToLower() == "teller")?.Value ??
                     (pid > 0 ? "ILM" : identity.Name);
        if (teller.ToLower() == "client" || teller.ToLower() == "app")
            teller = "ILM";
        var user = new User(teller, pid == 0 ? null : pid, identity.Claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value));
        return user;
    }
}