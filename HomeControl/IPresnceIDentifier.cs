using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BluetoothWatcherService
{
    public interface IPresnceIdentifier
    {
        void PersonConnected(string p);
        event EventHandler<string> PersonArrived;
        event EventHandler<string> PersonLeft;
        IDictionary<string, PersonState> getState();

        void registerPerson(string personName);
        void registerPerson(string personName, PersonStateConfiguration configuration);
    }
}
