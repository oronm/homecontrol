using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HomeControl.Local.Contracts;

namespace HomeControl.Local.App
{
    public interface IHomeController
    {
        event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived;
        event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft;
    }
}
