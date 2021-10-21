using Microsoft.IdentityModel.Claims;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsMVCTests
{
    [ExcludeFromCodeCoverage]
    public class CustomPrincipal : ClaimsPrincipal
    {
        public CustomPrincipal(List<Claim> claims) : base(new List<ClaimsIdentity>() { new CustomIdentity(claims) })
        {
        }

    }

    [ExcludeFromCodeCoverage]
    public class CustomIdentity : ClaimsIdentity
    {
        public CustomIdentity(List<Claim> claims) : base(claims)
        {
        }
    }
}
