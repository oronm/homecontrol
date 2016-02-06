using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Cloud.Model;
using log4net;

namespace HomeControl.Cloud.Managers
{
    // TODO : Replace with real distributed store
    public class StateStore : IStateStore
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IEnumerable<Realm> realms;
        ConcurrentDictionary<IndexKey, Person> peopleIndex;

        public StateStore()
        {
            InitRealms();
        }

        private void InitRealms()
        {
            this.realms = loadRealms();

            var flat =
                from realm in realms
                from rgroup in realm.groups
                from location in rgroup.Locations
                from person in location.People
                select new Tuple<IndexKey, Person>(
                        new IndexKey(realm.RealmName, rgroup.GroupName, location.LocationName, person.Name),
                        person);

            log.InfoFormat("flat {0}", flat.Count());

            this.peopleIndex = new ConcurrentDictionary<IndexKey,Person>(flat.ToDictionary(
                (realmKey) => realmKey.Item1, 
                (realmValue) => realmValue.Item2));
            log.InfoFormat("index {0}", peopleIndex.Count());
            foreach (var item in peopleIndex)
            {
                log.InfoFormat("index {0} {1}", item.Key, item.Value.Name);
            }
        }

        // TODO : Load realms from somewhere
        private IEnumerable<Realm> loadRealms()
        {
            return new Realm[] {
                new Realm("Default", new Group[] { 
                    new Group("Morad", new Location[] { 
                        new Location("Home", new Person[] { 
                            new Person("Galia"),
                            new Person("Oron")                        
                        })})})};
        }
        
        public void UpdateLocationState(string Realm, string Group, string Location, IEnumerable<Model.Person> people)
        {
            foreach (var person in people)
            {
                UpdatePersonState(Realm, Group, Location, person);   
            }
        }

        public void UpdatePersonState(string Realm, string Group, string Location, Model.Person person)
        {
            Person personInState;
            var key = new IndexKey(Realm, Group, Location, person.Name);
            key = peopleIndex.Keys.First(k => k.Person == "Oron");
            log.InfoFormat("updateperssta {0}", key);
            log.InfoFormat("updateperssta index {0}", peopleIndex.Count);
            if (peopleIndex.TryGetValue(key, out personInState))
            {
                log.InfoFormat("updateperssta person old {0}", personInState.Name);
                personInState.LastLeft = person.LastLeft;
                personInState.LastSeen = person.LastSeen;
                personInState.IsPresent = person.IsPresent;
            }
        }


        public IEnumerable<Person> GetLocationState(string Realm, string Group, string Location)
        {
            log.Info("getlocstatest");
            var realm = realms.SingleOrDefault(rlm => rlm.RealmName == Realm);
            log.InfoFormat("realm {0} {1} {2}", Realm, realms.First().RealmName, realm.RealmName);
            if (realm == null) return null;

            var group = realm.groups.SingleOrDefault(grp => grp.GroupName == Group);
            log.InfoFormat("gr {0} {1} {2}", Group, realm.groups.First().GroupName, group.GroupName);
            if (group == null) return null;

            var loc = group.Locations.SingleOrDefault(lc => lc.LocationName == Location);
            log.InfoFormat("loc {0} {1} {2}", Location, group.Locations.First().LocationName, loc.LocationName);
            if (loc == null) return null;

            log.InfoFormat("locpeople {0} {1}", loc.People.Count(), loc.People.First().Name);
            return loc.People.ToArray();
        }
    }

    class IndexKey
    {
        public string Realm;
        public string Group;
        public string Location;
        public string Person;

        public IndexKey(string realm, string group, string location, string person)
        {
            Realm = realm;
            Group = group;
            Location = location;
            Person = person;
        }

        public override string ToString()
        {
            return string.Concat(Realm, ",", Group, ",", Location, ",", Person);
        }
    }
}
