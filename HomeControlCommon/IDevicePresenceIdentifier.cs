using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Common
{
    public interface IDevicePresenceIdentifier
    {
        IDeviceDetails RegisterDevice(IDeviceDetails deviceDetails);
        bool UnregisterDevice(IDeviceDetails deviceDetails, out IDeviceDetails removedDeviceDetails);
        event EventHandler<DeviceIdentifiedEventArgs> OnDeviceIdentified;
    }
}
