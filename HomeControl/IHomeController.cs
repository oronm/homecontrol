using System;
namespace BluetoothListenerService
{
    public interface IHomeController
    {
        void NotifyGaliaArrivedHome();
        void NotifyGaliaLeftHome();
        void NotifyOronArrivedHome();
        void NotifyOronLeftHome();
    }
}
