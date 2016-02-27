using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeControl.Cloud.Contracts.Models
{
    public class StateIdentity
    {
        public string Realm;
        public string Group;
        public string Location;

        public StateIdentity(string Realm, string Group, string Location)
        {
            this.Realm = Realm;
            this.Group = Group;
            this.Location = Location;
        }
    }
}