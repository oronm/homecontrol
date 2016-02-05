using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WifiDeviceIdentifier;
using HomeControl.Detection;

namespace HomeControl.PresenceManager
{
    public interface ISensorFactory
    {
        ISensor GetOrCreate<T>(T detectable) where T : IDetectable;
    }

    public class SingletonsSensorFactory : ISensorFactory
    {
        private IDictionary<Type, ISensor> sensors;

        public SingletonsSensorFactory(WifiDeviceSensor wifiDeviceIdentifier)
        {
            sensors = new Dictionary<Type, ISensor>() {
                { typeof(WifiDeviceDetails), wifiDeviceIdentifier }
            };
        }

        public ISensor GetOrCreate<T>(T detectable) where T : IDetectable
        {
            var detectableType = detectable.GetType();
            if (sensors.ContainsKey(detectableType)) return sensors[detectableType];
            else return null;
        }
    }
}
