using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Cloud.Model
{
    public class Group
    {
        public string GroupName { get; private set; }
        public IEnumerable<Location> Locations { get; private set; }

        public Group(string GroupName, Location[] location)
        {
            this.GroupName = GroupName;
            this.Locations = location;
        }
    }
}
