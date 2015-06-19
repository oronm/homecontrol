using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HomeControl.Common;

namespace HomeControl.Common
{
    public class DeviceIdentifiedEventArgs
    {
        public IDeviceDetails deviceDeatils;
        public DateTime presenceUtcTime;
    }
}
