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
        public DateTime LastSeen;
        public DateTime LastLeft;
        public bool IsPresent;

        public Person(string name)
        {
            this.Name = name;
            LastSeen = DateTime.UtcNow.AddYears(-100);
            LastLeft = DateTime.UtcNow.AddYears(-100);
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
