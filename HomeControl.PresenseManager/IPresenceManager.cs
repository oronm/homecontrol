using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeControl.PresenceManager
{
    public interface IPresenceManager
    {
        event EventHandler<string> PersonArrived;
        event EventHandler<string> PersonLeft;
        IDictionary<string, PersonState> GetState();

        void RegisterPerson(PersonRegistration registration);
    }
}
