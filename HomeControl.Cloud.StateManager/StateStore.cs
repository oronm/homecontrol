﻿using System;
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
        private const int MAX_HISTORY = 10;

        IEnumerable<Realm> realms;
        ConcurrentDictionary<IndexKey, Person> peopleIndex;
        ConcurrentDictionary<IndexKey, ConcurrentQueue<PersonHistory>> peopleHistory;


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

            this.peopleHistory = new ConcurrentDictionary<IndexKey,ConcurrentQueue<PersonHistory>> (peopleIndex.Select((kvp,index) => 
                new KeyValuePair<IndexKey,ConcurrentQueue<PersonHistory>>(kvp.Key, new ConcurrentQueue<PersonHistory>())));

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

        public void UpdatePersonState(string Realm, string Group, string Location, Person person)
        {
            Person personInState;
            var key = new IndexKey(Realm, Group, Location, person.Name);
            log.DebugFormat("updateperssta {0}", key);
            log.DebugFormat("updateperssta index {0}", peopleIndex.Count);
            
            key = peopleIndex.Keys.FirstOrDefault(k => k.ToString() == key.ToString());
            if (key == null || !peopleIndex.TryGetValue(key, out personInState))
            {
                log.WarnFormat("Couldnt find key {0}", key == null ? "null" : key.ToString());
            }
            else
            {
                if (personInState.LastLeft > person.LastLeft ||
                    personInState.LastSeen > person.LastSeen)
                {
                    log.WarnFormat("Received old information about {0} with {1}", personInState, person);
                }
                else
                {
                    log.DebugFormat("cloning  {0}", personInState.Name);
                    var oldPersonInState = personInState.Clone();
                    log.DebugFormat("updateperssta person old {0}", personInState.Name);
                    personInState.LastLeft = person.LastLeft;
                    personInState.LastSeen = person.LastSeen;
                    personInState.IsPresent = person.IsPresent;

                    if (oldPersonInState.IsPresent != person.IsPresent &&
                        (oldPersonInState.LastLeft != DateTime.MinValue.ToUniversalTime() && oldPersonInState.LastSeen != DateTime.MinValue.ToUniversalTime()))
                    {
                        addHistoryRecord(key, oldPersonInState);
                    }
                    else
                    {
                        log.WarnFormat("update doesnt qualify history {0} {1} {2} {3}", oldPersonInState.IsPresent, person.IsPresent, oldPersonInState.LastLeft, oldPersonInState.LastSeen);
                    }
                }
            }
        }

        private void addHistoryRecord(IndexKey key, Person personForHistory)
        {
            ConcurrentQueue<PersonHistory> personHistory = null;
            if (peopleHistory.TryGetValue(key, out personHistory))
            {
                var historyRecord = new PersonHistory(DateTime.UtcNow, personForHistory);

                // Cleanup extra items
                int removals = personHistory.Count - MAX_HISTORY;
                while (personHistory.Count > MAX_HISTORY && removals > 0)
                {
                    removals --;
                    PersonHistory dummy;
                    personHistory.TryDequeue(out dummy);
                }

                // if there is room, add the item
                if (personHistory.Count < MAX_HISTORY)
                {
                    personHistory.Enqueue(historyRecord);
                }
            }
        }


        public IEnumerable<Person> GetLocationState(string Realm, string Group, string Location)
        {
            log.Debug("getlocstatest");
            var realm = realms.SingleOrDefault(rlm => rlm.RealmName == Realm);
            log.DebugFormat("realm {0} {1} {2}", Realm, realms.First().RealmName, realm.RealmName);
            if (realm == null) return null;

            var group = realm.groups.SingleOrDefault(grp => grp.GroupName == Group);
            log.DebugFormat("gr {0} {1} {2}", Group, realm.groups.First().GroupName, group.GroupName);
            if (group == null) return null;

            var loc = group.Locations.SingleOrDefault(lc => lc.LocationName == Location);
            log.DebugFormat("loc {0} {1} {2}", Location, group.Locations.First().LocationName, loc.LocationName);
            if (loc == null) return null;

            log.DebugFormat("locpeople {0} {1}", loc.People.Count(), loc.People.First().Name);
            return loc.People.ToArray();
        }


        public IEnumerable<PersonHistory> GetPersonStateHistory(string Realm, string Group, string Location, string Name)
        {
            ConcurrentQueue<PersonHistory> history;
            var key = new IndexKey(Realm, Group, Location, Name);
            log.DebugFormat("getpersonstatehistory {0}", key);

            key = peopleHistory.Keys.FirstOrDefault(k => k.ToString() == key.ToString());

            IEnumerable<PersonHistory> result;
            if (key == null || !peopleHistory.TryGetValue(key, out history))
            {
                log.WarnFormat("Couldnt find key {0}", key == null ? "null" : key.ToString());
                result = new PersonHistory[] { };
            }
            else
            {
                result = history.Take(MAX_HISTORY).ToArray();
            }

            return result;
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
