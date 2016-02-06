using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HomeControl.Cloud.Model;

namespace HomeControl.Cloud.Managers
{
    public interface IStateStore
    {
        void UpdateLocationState(string Realm, string Group, string Location, IEnumerable<Person> people);
        void UpdatePersonState(string Realm, string Group, string Location, Person person);
    }
}
