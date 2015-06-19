using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var devices = this.registeredDevices.Select((kvp) => kvp.Value);
            IEnumerable<string> connectedMACs = getConnectedMacs();
            return devices.Where(deviceInfo => connectedMACs.Contains(deviceInfo.DeviceId));
        }

        private IEnumerable<string> getConnectedMacs()
        {
            string urlAddress = "http://192.168.0.1/";
            string myParameters = "param1=value1&param2=value2&param3=value3";

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Credentials = new NetworkCredential("slim", "slimer1");
                string HtmlResult = wc.UploadString(urlAddress, myParameters);
            }

            return null;
        }
    }
}
