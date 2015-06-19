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
        private IDictionary<Type, IDevicePresenceIdentifier> creators = 
            new Dictionary<Type, IDevicePresenceIdentifier>() {
            { typeof(WifiDeviceDetails), new  WifiDevicePresenceIdentifier() }
        };

        public IDevicePresenceIdentifier Create<T>(T deviceDetails) where T : IDeviceDetails
        {
            if (creators.ContainsKey(typeof(T))) return creators[typeof(T)];
            else return null;
        }
    }
}
