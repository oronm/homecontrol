using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeControl.Local.Contracts
{
    public class PersonPresenceChangedEventArgs
    {
        public DateTime ChangeTimeUtc;
        public string Name;

        public PersonPresenceChangedEventArgs(HomeControl.PersonPresenceChangedEventArgs e)
        {
            this.ChangeTimeUtc = e.ChangeTimeUtc;
            this.Name = e.Name;
        }
    }

    public class PersonState
    {
        public string name;
        public DateTime lastSeen;
        public DateTime lastLeft;
        public bool IsPresent;
    }
}
