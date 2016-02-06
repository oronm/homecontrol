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
    public class StateManager : IStateFeed
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


        public void test(string name)
        {
        }
    }
}
