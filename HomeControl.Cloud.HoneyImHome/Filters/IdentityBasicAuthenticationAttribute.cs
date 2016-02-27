using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HomeControl.Cloud.HoneyImHome.Controllers;
using HomeControl.Cloud.Contracts.Models;

namespace WebApiBasicAuth.Filters
{
    // TODO : UGLY!! Change This!!
    public static class StateControllerExtentionMethods
    {
        private const string KEY = "Identity";
        public static StateIdentity GetFeederIdentity(this StateController controller)
        {
            StateIdentity result = null;
            if (HttpContext.Current.Items.Contains(KEY))
            {
                result = HttpContext.Current.Items[KEY] as StateIdentity;
            }

            return result;
        }

        public static void SetFeederIdentity(StateIdentity identity)
        {
            HttpContext.Current.Items.Add(KEY, identity);
        }
    }
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StateIdentity requestIdentity = null;

            if (string.IsNullOrWhiteSpace(token))
            {
                // No user with userName/password exists.
                return null;
            }
            else
            {
                requestIdentity = HomeControl.Cloud.HoneyImHome.WebApiApplication.TokensStore.GetIdentity(token);
                if (requestIdentity == null)
                    return null;

                StateControllerExtentionMethods.SetFeederIdentity(requestIdentity);
            }


            // Create a ClaimsIdentity with all the claims for this user.
            Claim nameClaim = new Claim(ClaimTypes.Authentication, token);
            List<Claim> claims = new List<Claim> { nameClaim };

            // important to set the identity this way, otherwise IsAuthenticated will be false
            // see: http://leastprivilege.com/2012/09/24/claimsidentity-isauthenticated-and-authenticationtype-in-net-4-5/
            ClaimsIdentity identity = new ClaimsIdentity(claims, System.Security.Claims.AuthenticationTypes.Basic);

            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

    }
}
