using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace HomeControl.Local
{
    public class StateReportersFactory : IStateReportersFactory
    {
        private readonly IStateReporter[] reporters;
        public StateReportersFactory()
        {
            var httpReporter = new HttpStateReporter();
            httpReporter.Init(new Dictionary<string, string>
            {
                { "endpoint", AppSettings.GetValue("HttpReporter.endpoint", "http://homecontrol-cloud-feedmanager.azurewebsites.net/api/State") },
                { "username", AppSettings.GetValue("HttpReporter.username", "username") },
                { "password", AppSettings.GetValue("HttpReporter.password", "password") }
            });

            reporters = new IStateReporter[] { httpReporter };
        }
        public IEnumerable<IStateReporter> GetRegisteredStateReporters()
        {
            return this.reporters;
        }
    }
}
