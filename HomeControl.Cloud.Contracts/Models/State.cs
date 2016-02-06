using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeControl.Cloud.Contracts.Models
{
    public class PersonState
    {
        public string name;
        public DateTime lastSeen;
        public DateTime lastLeft;
        public bool IsPresent;
    }

    public abstract class FeedMessage
    {
        public string Realm;
        public string Group;
        public string Location;
    }

    public class UpdateLocationState : FeedMessage
    {
        public IEnumerable<PersonState> MembersState;
    }

    public class UpdatePersonState : FeedMessage
    {
        public PersonState MemberState;
    }
}