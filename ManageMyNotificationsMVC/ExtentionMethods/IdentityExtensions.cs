using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ManageMyNotificationsMVC
{
    [ExcludeFromCodeCoverage]
    public static class IdentityExtensions
    {

        public static string GetUserName(this System.Security.Principal.IPrincipal principalUser)
        {
            if (!VerifyImpersonationMode(principalUser))
            {
                var claims = ((Microsoft.IdentityModel.Claims.ClaimsIdentity)((Microsoft.IdentityModel.Claims.ClaimsPrincipal)principalUser).Identity).Claims;
                return claims.First(x => x.ClaimType == System.Security.Claims.ClaimTypes.Name).Value ?? string.Empty;
            }
            else
            {
                var impersonatedUser = GetImpersonatedUserDetails(principalUser);
                return !string.IsNullOrWhiteSpace(impersonatedUser) ? impersonatedUser.Split('|')[0].ToString() : string.Empty;
            }

        }
        public static string GetAdfsGuid(this System.Security.Principal.IPrincipal principalUser)
        {

            if (!VerifyImpersonationMode(principalUser))
            {
                var claims = ((Microsoft.IdentityModel.Claims.ClaimsIdentity)((Microsoft.IdentityModel.Claims.ClaimsPrincipal)principalUser).Identity).Claims;

                // Identity Server encodes the adfs Guid in this way for some reason...don't think Guids need to be encoded.
                string base64EncodedByteArrayAdfsGuid = claims.First(x => x.ClaimType == System.Security.Claims.ClaimTypes.NameIdentifier).Value ?? string.Empty;

                var adfsGuid = new Guid(Convert.FromBase64String(base64EncodedByteArrayAdfsGuid));
                return adfsGuid.ToString();
            }
            else
            {
                var impersonatedUser = GetImpersonatedUserDetails(principalUser);
                return !string.IsNullOrWhiteSpace(impersonatedUser) ? impersonatedUser.Split('|')[1].ToString() : string.Empty;
            }

        }

        public static bool VerifyImpersonationMode(this System.Security.Principal.IPrincipal principalUser)
        {
            var claims = ((Microsoft.IdentityModel.Claims.ClaimsIdentity)((Microsoft.IdentityModel.Claims.ClaimsPrincipal)principalUser).Identity).Claims;
            var impersonatedUserClaims = claims.Where(x => x.ClaimType.Equals("zImpersonatedUser")).FirstOrDefault();
            if (impersonatedUserClaims != null)
                return true;
            else
                return false;
        }

        public static string GetImpersonationMessage(this System.Security.Principal.IPrincipal principalUser)
        {
            var impersonatedUser = GetImpersonatedUserDetails(principalUser);
            return string.Format("You are impersonating the user: {0}", !string.IsNullOrWhiteSpace(impersonatedUser) ? impersonatedUser.Split('|')[0].ToString() : string.Empty);
        }

        private static string GetImpersonatedUserDetails(this System.Security.Principal.IPrincipal principalUser)
        {
            var claims = ((Microsoft.IdentityModel.Claims.ClaimsIdentity)((Microsoft.IdentityModel.Claims.ClaimsPrincipal)principalUser).Identity).Claims;
            var impersonatedUserClaims = claims.Where(x => x.ClaimType.Equals("zImpersonatedUser")).FirstOrDefault();
            return (impersonatedUserClaims != null && !string.IsNullOrWhiteSpace(impersonatedUserClaims.Value)) ? impersonatedUserClaims.Value : string.Empty;
        }
    }
}