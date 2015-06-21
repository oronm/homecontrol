using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeControlService
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
}
