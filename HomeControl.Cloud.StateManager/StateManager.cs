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
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IStateStore stateStore;

        // TODO : Convert StateManager to work with IOC
        public StateManager()
        {
            log.InfoFormat("st mgr created {0}", DateTime.Now.ToShortTimeString());
            this.stateStore = new StateStore();
        }
        public StateManager(IStateStore stateStore)
        {
            log.InfoFormat("st mgr created with store at {0}", DateTime.Now.ToShortTimeString());
            this.stateStore = stateStore;
        }

        public async Task Feed(UpdateLocationState newState)
        {
            if (newState == null) throw new ArgumentNullException("newState");
            IEnumerable<Person> people = newState.MembersState.Select(ps => CreatePerson(ps));
            Task.Run(() => stateStore.UpdateLocationState(newState.Realm, newState.Group, newState.Location, people));
        }

        public async Task Feed(UpdatePersonState newPersonState)
        {
            if (newPersonState == null) throw new ArgumentNullException("newPersonState");
            Task.Run(() => stateStore.UpdatePersonState(newPersonState.Realm, newPersonState.Group, newPersonState.Location, CreatePerson(newPersonState.MemberState)));
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
            log.Info("getlocstate");
            var people = stateStore.GetLocationState(Realm, Group, Location);
            log.InfoFormat("getlocstate people {0}", people == null ? "null" : people.Count().ToString());
            IEnumerable<PersonState> result = null;
            if (people != null)
            {
                result = people.Select(person => CreatePersonState(person));
                log.InfoFormat("getlocstate result {0}", result == null ? "null" : result.Count().ToString());
                foreach (var person in people)
                {
                    log.InfoFormat("person {0} {1} {2} {3}", person.Name, person.LastSeen.ToShortDateString(), person.LastLeft.ToShortDateString(), person.IsPresent);
                }
            }
            return result;
        }
    }
}
