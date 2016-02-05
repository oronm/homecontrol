using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeControl
{
    public interface IPresnceIdentifier
    {
        event EventHandler<string> PersonArrived;
        event EventHandler<string> PersonLeft;
        IDictionary<string, PersonState> getState();

        void registerPerson(PersonRegistration registration);
    }
}
