using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeControl.Local.Contracts
{
    public interface ILocalHomeControlService
    {
        event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived;
        event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft;

        void Start();
        bool Stop();

        IEnumerable<PersonState> GetState();
    }
}
