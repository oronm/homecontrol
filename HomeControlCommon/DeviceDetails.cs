using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Common
{
    public interface IDeviceDetails
    {
        string DeviceId { get; set; }
        string DeviceName { get; set; }
    }

    public class WifiDeviceDetails : IDeviceDetails
    {
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }
    }

}
