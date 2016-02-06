using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Cloud.Model
{
    public class Location
    {
        public string LocationName { private set; get; }
        public IEnumerable<Person> People { private set; get; }

        public Location(string LocationName, Person[] people)
        {
            this.LocationName = LocationName;
            this.People = people;
        }
    }
}
