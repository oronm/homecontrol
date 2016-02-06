using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Cloud.Model
{
    public class Person
    {
        public readonly string Name;
        public readonly DateTime LastSeen;
        public readonly DateTime LastLeft;
        public readonly bool IsPresent;

        public Person(string name)
        {
            this.Name = name;
            LastSeen = DateTime.MinValue;
            LastLeft = DateTime.MinValue;
            IsPresent = false;
        }

        public Person(string name, DateTime lastSeen, DateTime lastLeft, bool IsPresent)
        {
            this.Name = name;
            this.LastSeen = lastSeen;
            this.LastLeft = lastLeft;
            this.IsPresent = IsPresent;
        }
    }
}
