using System;
namespace HomeControl
{
    public interface IHomeController
    {
        event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived; 
        event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft; 
    }
}
