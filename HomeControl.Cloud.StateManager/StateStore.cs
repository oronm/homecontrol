using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Cloud.Model;

namespace HomeControl.Cloud.Managers
{
    public class StateStore : IStateStore
    {
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

            this.peopleIndex = new ConcurrentDictionary<IndexKey,Person>(flat.ToDictionary(
                (realmKey) => realmKey.Item1, 
                (realmValue) => realmValue.Item2));
        }

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
            Person oldValue;
            var key = new IndexKey(Realm, Group, Location, person.Name);
            if (peopleIndex.TryGetValue(key, out oldValue))
            {
                peopleIndex.TryUpdate(key, oldValue, person);
            }
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
    }
}
