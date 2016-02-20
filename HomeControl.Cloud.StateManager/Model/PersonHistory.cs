using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Cloud.Model;

namespace HomeControl.Cloud.Model
{
    public class PersonHistory : Person
    {
        public readonly DateTime RecordDateUTC;
        public PersonHistory(DateTime recordDateUTC, Person person) : base(person.Name, person.LastSeen, person.LastLeft, person.IsPresent)
        {
            this.RecordDateUTC = recordDateUTC;
        }
    }
}
