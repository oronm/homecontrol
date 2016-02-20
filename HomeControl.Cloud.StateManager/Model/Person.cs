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

        public override string ToString()
        {
            return string.Concat(Name, ",", LastSeen, ",", LastLeft, ",", IsPresent);
        }

        public Person(string name)
        {
            this.Name = name;
            LastSeen = DateTime.MinValue.ToUniversalTime();
            LastLeft = DateTime.MinValue.ToUniversalTime();
            IsPresent = false;
        }

        public Person(string name, DateTime lastSeen, DateTime lastLeft, bool IsPresent)
        {
            this.Name = name;
            this.LastSeen = lastSeen;
            this.LastLeft = lastLeft;
            this.IsPresent = IsPresent;
        }

        public Person Clone()
        {
            return new Person(this.Name) { IsPresent = this.IsPresent, LastLeft = this.LastLeft, LastSeen = this.LastSeen };
        }
    }
}
