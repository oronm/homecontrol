using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using HomeControl.Detection;

namespace HomeControl.PresenceManager
{
    public struct PersonRegistration
    {
        public string PersonName;
        public IEnumerable<IDetectable> Detectables;
        public PersonPresencePolicy PresencePolicy;
    }

    public class PersonPresencePolicy
    {
        public TimeSpan MaximumAbsencyAllowed;

        public PersonPresencePolicy()
        {
            MaximumAbsencyAllowed = TimeSpan.FromMinutes(AppSettings.GetValue("PresenceManager::PersonPresencePolicy::MaximumAbscencyAllowedMinutes", 10));
        }
    }

}

