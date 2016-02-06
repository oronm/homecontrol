using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Cloud.Contracts;
using HomeControl.Cloud.Contracts.Models;
using HomeControl.Cloud.Model;

namespace HomeControl.Cloud.Managers
{
    public class StateManager : IStateFeed, IStateReport
    {
        private IStateStore stateStore;
        public StateManager()
        {
            this.stateStore = new StateStore();
        }
        public StateManager(IStateStore stateStore)
        {
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

        public void test(string name)
        {
        }

        public IEnumerable<PersonState> GetLocationState(string Realm, string Group, string Location)
        {
            var people = stateStore.GetLocationState(Realm, Group, Location);
            IEnumerable<PersonState> result = null;
            if (people != null)
            {
                result = people.Select(person => CreatePersonState(person));
            }
            return result;
        }
    }
}
