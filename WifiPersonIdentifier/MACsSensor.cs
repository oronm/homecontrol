﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Detection;
using log4net;

namespace WifiDeviceIdentifier
{
    // TODO : wifidevice - Rename sensor to convetion + Add another level for tplink wifi sensor
    // TODO : wifidevice - use configuration instead of hard coded
    public class MACsSensor : AbstractTimedSensor, ISensor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override IEnumerable<IDetectable> Detect()
        {
            var devices = this.registeredDetectables.Select((kvp) => kvp.Value);

            log.DebugFormat("MACs Registered: {0}{1}", Environment.NewLine + "\t", 
                String.Join(Environment.NewLine + "\t", devices.Select(device => device.Identification + " " + device.Description)));

            IEnumerable<string> connectedMACs = getConnectedMacs();

            log.DebugFormat("MACs Found: {0}{1}", Environment.NewLine + "\t", String.Join(Environment.NewLine + "\t", connectedMACs));

            return devices.Where(deviceInfo => connectedMACs.Contains(parseMacFromString(deviceInfo.Identification)));
        }

        private IEnumerable<string> getConnectedMacs()
        {
            return (from ipinfo in IPInfo.GetIPInfo()
                    where !ipinfo.MacAddress.StartsWith("01-00")
                   select parseMacFromString(ipinfo.MacAddress)).ToArray();
        }

        private string parseMacFromString(string macAddressString)
        {
            if (string.IsNullOrWhiteSpace(macAddressString))
                return macAddressString;

            string[] stringsToRemove = { "\"", " ", ":", "-", @"\n", };
            foreach (var stringToRemove in stringsToRemove)
            {
                macAddressString = macAddressString.Replace(stringToRemove, string.Empty);
            }
            return macAddressString.Trim().ToLower();
        }
    }
}