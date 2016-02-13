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
using HomeControl.Cloud.Managers;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Configuration;
using log4net;

namespace HomeControl.Cloud.FeedManager.Controllers
{
    public class StateController : ApiController
    {
        private IStateFeed stateFeed;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO : Convert StateController to work with DI
        public StateController()
        {
            var fac = new ChannelFactory<IStateFeed>(new WebHttpBinding(), new EndpointAddress(ConfigurationManager.AppSettings["SVC.Managers.Endpoint"]));
            fac.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior() { DefaultOutgoingRequestFormat = WebMessageFormat.Json, DefaultOutgoingResponseFormat = WebMessageFormat.Json, DefaultBodyStyle = WebMessageBodyStyle.WrappedRequest });
            this.stateFeed = fac.CreateChannel();   
        }
        
        private bool validateIdentity(FeederIdentity identity)
        {
            return identity != null &&
                !string.IsNullOrWhiteSpace(identity.Group) &&
                !string.IsNullOrWhiteSpace(identity.Location) &&
                !string.IsNullOrWhiteSpace(identity.Realm);
        }
        private bool validateState(PersonState state)
        {
            var valid = state != null &&
                !string.IsNullOrWhiteSpace(state.name) &&
                state.lastLeft.ToUniversalTime() < DateTime.UtcNow && state.lastSeen.ToUniversalTime() < DateTime.UtcNow &&
                ((state.IsPresent && state.lastLeft < state.lastSeen) ||
                (!state.IsPresent && (state.lastLeft > state.lastSeen || (state.lastLeft == state.lastSeen && state.lastSeen == DateTime.MinValue.ToUniversalTime()))));

            if (valid)
            {
                state.lastLeft = state.lastLeft.ToUniversalTime();
                state.lastSeen = state.lastSeen.ToUniversalTime();
            }
            else
            {
                log.DebugFormat("Invalid state for {0}", state);
            }
            return valid;
        }
        private bool validateState(IEnumerable<PersonState> state)
        {
            return state.All(st => validateState(st));
        }

        [IdentityBasicAuthentication]
        [Authorize]
        // POST: api/State
        public async Task<IHttpActionResult> Post([FromBody]IEnumerable<PersonState> value)
        {
            //log.Debug("rec post");
            var identity = this.GetFeederIdentity();
            //log.Debug("rec post iden");
            if (!validateIdentity(identity)) return Unauthorized();
            //log.Debug("rec post auth");
            if (!validateState(value)) return BadRequest();
            //log.Debug("rec post valid");

            var feed = new UpdateLocationState()
            {
                Realm = identity.Realm,
                Group = identity.Group,
                Location = identity.Location,
                MembersState = value
            };

            await Task.Run(() => { log.DebugFormat("Feeding location state {0}", feed); stateFeed.Feed(feed); });
            return Ok();
        }

        // PUT: api/State/5
        [IdentityBasicAuthentication]
        [Authorize]
        public async Task<IHttpActionResult> Put(string name, [FromBody]PersonState value)
        {
            var identity = this.GetFeederIdentity();
            if (!validateIdentity(identity)) return Unauthorized();
            if (!validateState(value)) return BadRequest();

            var feed = new UpdatePersonState()
            {
                Realm = identity.Realm,
                Group = identity.Group,
                Location = identity.Location,
                MemberState = value
            };

            await Task.Run(() => { log.DebugFormat("Feeding person state {0}", feed); stateFeed.Feed(feed); });
            return Ok();
        }
    }
}
