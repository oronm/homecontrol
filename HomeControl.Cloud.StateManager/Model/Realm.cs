using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Cloud.Model
{
    public class Realm
    {
        public string RealmName { private set; get; }
        public IEnumerable<Group> groups { private set; get; }
        
        public Realm(string RealmName, Group[] groups)
        {
            this.RealmName = RealmName;
            this.groups = groups;
        }
    }
}
