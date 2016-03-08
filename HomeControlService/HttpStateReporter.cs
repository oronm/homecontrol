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
        private string endpoint;
        private string username;
        private string password;
        public void Init(IDictionary<string, string> configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            endpoint = extractConfigValue(configuration, "endpoint");
            username = extractConfigValue(configuration, "username");
            password = extractConfigValue(configuration, "password");
        }

        private string extractConfigValue(IDictionary<string, string> configuration, string key)
        {
            string value;
            if (!configuration.TryGetValue(key, out value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(key, string.Format("Configuration doesnt containt {0}", key));
            }
            return value;
        }

        private void report(Func <HttpClient, Task<HttpResponseMessage>> reportAction)
        {
            using (var client = new HttpClient())
            {
                var authentication = Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", username, password)));

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
