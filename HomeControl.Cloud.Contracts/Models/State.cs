using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HomeControl.Cloud.Contracts.Models
{
    [DataContract]
    public class PersonState
    {
        [DataMember]
        public string name;
        [DataMember]
        public DateTime lastSeen;
        [DataMember]
        public DateTime lastLeft;
        [DataMember]
        public bool IsPresent;
    }

    [DataContract]
    public abstract class FeedMessage
    {
        [DataMember]
        public string Realm;
        [DataMember]
        public string Group;
        [DataMember]
        public string Location;
    }

    [DataContract]
    public class UpdateLocationState : FeedMessage
    {
        [DataMember]
        public IEnumerable<PersonState> MembersState;
    }

    [DataContract]
    public class UpdatePersonState : FeedMessage
    {
        [DataMember]
        public PersonState MemberState;
    }
}