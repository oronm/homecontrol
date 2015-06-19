using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Common;

namespace WifiDeviceIdentifier
{
    public class WifiDevicePresenceIdentifier : AbstractTimedDevicePresenceIDentifier, IDevicePresenceIdentifier
    {
        protected override IEnumerable<IDeviceDetails> IdentifyDevicesPresence()
        {
            return null;
        }
    }
}
