using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Detection
{
    public interface IDetectable
    {
        string Identification { get; }
        string Description { get; }
    }

    public class WifiDeviceDetails : IDetectable
    {
        private readonly string deviceName;
        private readonly string MACAddress;

        public WifiDeviceDetails(string deviceName, string macAddress)
        {
            this.deviceName = deviceName;
            this.MACAddress = macAddress;
        }

        public string Identification { get { return MACAddress; } }

        public string Description { get { return this.deviceName; } }
    }

}
