using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HomeControl.Cloud.FeedManager.Controllers;
using HomeControl.Cloud.Contracts.Models;

namespace WebApiBasicAuth.Filters
{
    public static class FeederControllerExtentionMethods
    {
        private const string KEY = "FeederIdentity";
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
            HttpContext.Current.Items.Add("FeederIdentity", identity);
        }
    }
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); 

            if (string.IsNullOrWhiteSpace(token))
            {
                // No user with userName/password exists.
                return null;
            }
            if (token == "oron:oron")
            {
                FeederControllerExtentionMethods.SetFeederIdentity(new StateIdentity("Default", "Morad", "Home"));
            }
            else if (token == "yarimi:efes")
            {
                FeederControllerExtentionMethods.SetFeederIdentity(new StateIdentity("Default", "Yarimi", "Home"));
            }
            else
            {
                return null;
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
