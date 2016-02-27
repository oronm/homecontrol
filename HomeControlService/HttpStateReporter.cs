using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Claims;
using System.Net.Http.Headers;

namespace HomeControl.Local
{
    public class HttpStateReporter : IStateReporter
    {
        private const string ENDPOINT_CONFIGURATION_KEY = "endpoint";
        private string endpoint;
        public void Init(IDictionary<string, string> configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (!configuration.TryGetValue(ENDPOINT_CONFIGURATION_KEY, out endpoint) || string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentNullException(ENDPOINT_CONFIGURATION_KEY, "Configuration doesnt containt key");
            }
        }

        private void report(Func <HttpClient, Task<HttpResponseMessage>> reportAction)
        {
            using (var client = new HttpClient())
            {
                var authentication = Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "oron", "oron")));

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(AuthenticationTypes.Basic, authentication);

                reportAction(client).Result.EnsureSuccessStatusCode();//.ContinueWith( (res) => res.Result.EnsureSuccessStatusCode());
            }
        }

        public async Task ReportPersonState(Contracts.PersonState personState)
        {
            //Task.Run(() => report((client) => 
            //    client.PutAsJsonAsync(string.Concat(endpoint,"/", personState.name,"/"), personState)
            //));
        }
        
        public async Task ReportLocationState(IEnumerable<Contracts.PersonState> peopleState)
        {
            Task.Run(() => report((client) => client.PostAsJsonAsync(endpoint, peopleState)));
        }
    }
}
