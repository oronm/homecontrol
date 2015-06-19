using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl
{

    public class PersonState
    {
        public string name;
        public DateTime lastSeen;
        public PersonStateConfiguration configuration;
        public PersonState(string name, PersonStateConfiguration configuration)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            this.name = name;
            this.configuration = configuration;
            lastSeen = DateTime.MinValue;
        }
        public bool IsPresent()
        {
            return (DateTime.UtcNow - lastSeen) <= (configuration.MaximumAllowedDisconnection);
        }
    }
}
