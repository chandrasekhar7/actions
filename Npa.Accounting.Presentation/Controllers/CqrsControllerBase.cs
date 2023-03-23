using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Npa.Accounting.Common.ErrorHandling;

namespace Npa.Accounting.Presentation.Controllers
{
    public abstract class CqrsControllerBase : ControllerBase
    {
        public CqrsControllerBase()
        {
        }

        protected int GetAspNetUsersId() =>
            GetClaimsValue(ClaimTypes.NameIdentifier, value => int.TryParse(value, out var id) ? id : throw new AuthorizationException("Can't retrieve user ID for anonymous user."));

        private T GetClaimsValue<T>(string claimName, Func<string, T> parseClaimValue)
        {
            var claim = User.FindFirst(claimName);
            if (claim != null)
            {
                return parseClaimValue(claim.Value);
            }
            return default;
        }
    }
}