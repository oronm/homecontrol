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
    [EnableCors(origins: "http://localhost:60392", headers: "*", methods: "*")]
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
        
        private bool validateIdentity(FeederIdentity identity)
        {
            return identity != null &&
                !string.IsNullOrWhiteSpace(identity.Group) &&
                !string.IsNullOrWhiteSpace(identity.Location) &&
                !string.IsNullOrWhiteSpace(identity.Realm);
        }

        //[IdentityBasicAuthentication]
        //[Authorize]
        // POST: api/State
        public async Task<IEnumerable<PersonState>> Get()
        {
            var id = new
            {
                Realm = "Default",
                Group = "Morad",
                Location = "Home"
            };
         
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
    }
}
