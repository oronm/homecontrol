using System;
using System.Collections.Generic;
using HomeControl.PresenceManager;
namespace HomeControl
{
    public interface IHomeController
    {
        event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived; 
        event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft;
        IEnumerable<PersonState> GetState();
    }
}
