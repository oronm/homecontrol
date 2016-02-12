using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                { "endpoint", ConfigurationManager.AppSettings["HttpReporter.endpoint"] }
            });

            reporters = new IStateReporter[] { httpReporter };
        }
        public IEnumerable<IStateReporter> GetRegisteredStateReporters()
        {
            return this.reporters;
        }
    }
}
