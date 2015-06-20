using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WifiDeviceIdentifier;

namespace HomeControl.Common
{
    public interface IDevicePresenceFactory
    {
        IDevicePresenceIdentifier Create<T>(T deviceDetails) where T : IDeviceDetails;
    }

    public class DevicePresenceFactory : IDevicePresenceFactory
    {
        private IDictionary<Type, IDevicePresenceIdentifier> creators;

        public DevicePresenceFactory(WifiDevicePresenceIdentifier wifiDeviceIdentifier)
        {
            creators = new Dictionary<Type, IDevicePresenceIdentifier>() {
                { typeof(WifiDeviceDetails), wifiDeviceIdentifier }
            };
        }

        public IDevicePresenceIdentifier Create<T>(T deviceDetails) where T : IDeviceDetails
        {
            var detailsType = deviceDetails.GetType();
            if (creators.ContainsKey(detailsType)) return creators[detailsType];
            else return null;
        }
    }
}
