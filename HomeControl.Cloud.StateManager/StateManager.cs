using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Cloud.Contracts;
using HomeControl.Cloud.Contracts.Models;
using HomeControl.Cloud.Model;
using log4net;

namespace HomeControl.Cloud.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class StateManager : IStateFeed, IStateReport
    {
        private static ILog log;
        private IStateStore stateStore;

        // TODO : Convert StateManager to work with IOC
        public StateManager()
        {
            StateManager.log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.InfoFormat("st mgr created {0}", DateTime.Now.ToShortTimeString());
            this.stateStore = new StateStore();
        }
        public StateManager(IStateStore stateStore)
        {
            StateManager.log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.InfoFormat("st mgr created with store at {0}", DateTime.Now.ToShortTimeString());
            this.stateStore = stateStore;
        }

        public async Task Feed(UpdateLocationState newState)
        {
            log.DebugFormat("got location state {0}", newState == null ? "null" : newState.ToString());
            if (newState == null) throw new ArgumentNullException("newState");
            IEnumerable<Person> people = newState.MembersState.Select(ps => CreatePerson(ps));
            Task.Run(() => { 
                try
                {
                    stateStore.UpdateLocationState(newState.Realm, newState.Group, newState.Location, people);
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Error updating state for {0} {1} {2}", newState.Realm, newState.Group, newState.Location), e);
                }
            });
        }

        public async Task Feed(UpdatePersonState newPersonState)
        {
            log.DebugFormat("got person state {0}", newPersonState == null ? "null" : newPersonState.ToString());
            if (newPersonState == null) throw new ArgumentNullException("newPersonState");
            Task.Run(() => {
                try
                { 
                    stateStore.UpdatePersonState(newPersonState.Realm, newPersonState.Group, newPersonState.Location, CreatePerson(newPersonState.MemberState));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Error updating person state for {0}", newPersonState.MemberState.name), e);
                }
            });
        }

        private Person CreatePerson(PersonState personState)
        {
            return new Person(personState.name, personState.lastSeen, personState.lastLeft, personState.IsPresent);
        }
        private PersonState CreatePersonState(Person person)
        {
            return new PersonState(person.Name, person.LastSeen, person.LastLeft, person.IsPresent);
        }

        public IEnumerable<PersonState> GetLocationState(string Realm, string Group, string Location)
        {
            log.Debug("getlocstate");
            var people = stateStore.GetLocationState(Realm, Group, Location);
            log.DebugFormat("getlocstate people {0}", people == null ? "null" : people.Count().ToString());
            IEnumerable<PersonState> result = null;
            if (people != null)
            {
                result = people.Select(person => CreatePersonState(person));
                log.DebugFormat("getlocstate result {0}", result == null ? "null" : result.Count().ToString());
                foreach (var person in people)
                {
                    log.DebugFormat("person {0} {1} {2} {3}", person.Name, person.LastSeen.ToShortDateString(), person.LastLeft.ToShortDateString(), person.IsPresent);
                }
            }
            return result;
        }


        public PersonStateHistory GetPersonHistory(string Realm, string Group, string Location, string Name)
        {
            var history = stateStore.GetPersonStateHistory(Realm, Group, Location, Name);
            return new PersonStateHistory() { 
                name = Name,
                history = history.Select(ph => new PersonStateHistoryRecord() { 
                    recordDate = ph.RecordDateUTC,
                    lastLeft = ph.LastLeft,
                    lastSeen = ph.LastSeen,
                    wasPresent = ph.IsPresent
                })
            };
        }
    }
}
