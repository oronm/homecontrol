﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.PresenceManager
{

    public class PersonState
    {
        public string name;
        public DateTime lastSeen;
        public DateTime lastLeft;
        private PersonPresencePolicy configuration;

        internal PersonState(string name, PersonPresencePolicy configuration)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            this.name = name;
            this.configuration = configuration;
            lastSeen = DateTime.MinValue;
            lastLeft = DateTime.MinValue;
        }
        public bool IsPresent()
        {
            return (DateTime.UtcNow - lastSeen) <= (configuration.MaximumAbsencyAllowed);
        }
    }
}
