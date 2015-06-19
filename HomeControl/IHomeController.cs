using System;
namespace HomeControl
{
    public interface IHomeController
    {
        void NotifyGaliaArrivedHome();
        void NotifyGaliaLeftHome();
        void NotifyOronArrivedHome();
        void NotifyOronLeftHome();
    }
}
