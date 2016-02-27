using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using HomeControl.Cloud.Contracts;
using HomeControl.Cloud.Contracts.Models;
using WebApiBasicAuth.Filters;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Configuration;
using log4net;
using System.Web.Http.Cors;

namespace HomeControl.Cloud.HoneyImHome.Controllers
{
    public class StateController : ApiController
    {
        private  IStateReport stateReport;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO : Convert StateController to work with DI
        public StateController()
        {
            var fac = new ChannelFactory<IStateReport>(new WebHttpBinding(), new EndpointAddress(ConfigurationManager.AppSettings["SVC.Report.Endpoint"]));
            fac.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior() { DefaultOutgoingRequestFormat = WebMessageFormat.Json, DefaultOutgoingResponseFormat = WebMessageFormat.Json, DefaultBodyStyle = WebMessageBodyStyle.Wrapped });
            this.stateReport = fac.CreateChannel();   
        }
        
        private bool validateIdentity(StateIdentity identity)
        {
            return identity != null &&
                !string.IsNullOrWhiteSpace(identity.Group) &&
                !string.IsNullOrWhiteSpace(identity.Location) &&
                !string.IsNullOrWhiteSpace(identity.Realm);
        }

        [IdentityBasicAuthentication]
        [Authorize]
        // POST: api/State
        public async Task<IEnumerable<PersonState>> Get()
        {
            var id = this.GetFeederIdentity();
            if (id == null) return null;
         
            log.DebugFormat("Getting state");
            IEnumerable<PersonState> res = null;
            try
            {
                res = stateReport.GetLocationState(id.Realm, id.Group, id.Location);
            }
            catch (Exception e)
            {
                res = new PersonState[] {};
            }
            return res;
        }

        [Route("api/State/{name}/History")]
        [IdentityBasicAuthentication]
        [Authorize]
        public async Task<PersonStateHistory> GetHistory(string name)
        {
            var id = this.GetFeederIdentity();
            if (id == null) return null;

            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            PersonStateHistory history;
            try
            {
                history = stateReport.GetPersonHistory(id.Realm, id.Group, id.Location, name);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error getting history for {0}", name), e);
                history = new PersonStateHistory() { name = name, history = new PersonStateHistoryRecord[] { } };
            }

            return history;
        }

        [Route("api/State/CreateToken")]
        [HttpGet]
        public async Task<string> CreateToken(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            string token = null;

            if (username == "oron" && password == "oron")
            {
                token = WebApiApplication.TokensStore.CreateToken(new StateIdentity("Default", "Morad", "Home"));
            }
            else if (username == "yarimi" && password == "efes")
            {
                token = WebApiApplication.TokensStore.CreateToken(new StateIdentity("Default", "Yarimi", "Home"));
            }

            return token;
        }

        [HttpOptions]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        [HttpOptions]
        [HttpPost]
        [Route("api/State/CreateTokenEmail")]
        public IHttpActionResult CreateTokenEmail(LoginRequest req)
        {
            var token = createToken(req.email, req.password);
            if (string.IsNullOrWhiteSpace(token))
            {
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid login details" } );
            }
            else
            {
                return Ok(new { access_token = token });
            }
        }

        [Route("api/State/CreateTokenEmail")]
        [HttpPost]
        [HttpGet]
        public async Task<string> CreateTokenEmail(string email, string password)
        {
            return createToken(email, password);
        }

        private string createToken(string id, string password)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(password))
                return null;

            string token = null;

            if (id == "oron" && password == "oron")
            {
                token = WebApiApplication.TokensStore.CreateToken(new StateIdentity("Default", "Morad", "Home"));
            }
            else if (id == "yarimi" && password == "efes")
            {
                token = WebApiApplication.TokensStore.CreateToken(new StateIdentity("Default", "Yarimi", "Home"));
            }

            return token;
        }
    }

    public struct LoginRequest
    {
        public string email;
        public string password;
    }
}
