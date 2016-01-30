using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Common
{
    public interface IDeviceDetails
    {
        string DeviceId { get; }
        string DeviceName { get; }
    }

    public class WifiDeviceDetails : IDeviceDetails
    {
        private readonly string deviceName;
        private readonly string MACAddress;

        public WifiDeviceDetails(string deviceName, string macAddress)
        {
            this.deviceName = deviceName;
            this.MACAddress = macAddress;
        }

        public string DeviceId { get { return MACAddress; } }

        public string DeviceName { get { return this.deviceName; } }
    }

}
