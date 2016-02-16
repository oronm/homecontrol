using System;
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
    public class WifiDeviceSensor : AbstractTimedSensor, ISensor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        protected override IEnumerable<IDetectable> Detect()
        {
            var devices = this.registeredDetectables.Select((kvp) => kvp.Value);
            log.DebugFormat("MACs Registered: {0}{1}", Environment.NewLine + "\t", String.Join(Environment.NewLine + "\t", devices.Select(device => device.Identification + " " + device.Description)));
            IEnumerable<string> connectedMACs = getConnectedMacs();
            log.DebugFormat("MACs Found: {0}{1}", Environment.NewLine+"\t", String.Join(Environment.NewLine+"\t", connectedMACs));
            return devices.Where(deviceInfo => connectedMACs.Contains(parseMacFromString(deviceInfo.Identification)));
        }

        private IEnumerable<string> getConnectedMacs()
        {
            string urlAddress = "http://192.168.0.1/userRpm/WlanStationRpm_5g.htm?Page=1";
            IEnumerable<string> macs = null;
            
            try
            {
                var html = getWifiStatsHtmlPage(urlAddress);
                if (!string.IsNullOrWhiteSpace(html))
                {
                    var rawMacsArray = extractMacsRawAray(html);
                    if (!string.IsNullOrWhiteSpace(rawMacsArray))
                    {
                        macs = parseMacsFromRawArray(rawMacsArray);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error getting wifi stats html page from url {0}", urlAddress), e);
            }
            
            return macs;
        }

        private IEnumerable<string> parseMacsFromRawArray(string macsArray)
        {
            IEnumerable<string> array = null;
            array = macsArray.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

            if (array.Count() <= 2) return array;
            array = array.Take(array.Count() - 2).ToArray();
            if (array.Count() == 0) return array;

            array = array.Where((element, index) =>
            {
                return index % 4 == 0;
            }).Select(el => parseMacFromString(el)).ToArray();

            return array;
        }

        private string getWifiStatsHtmlPage(string urlAddress)
        {
            HttpWebRequest request = HttpWebRequest.Create(urlAddress) as HttpWebRequest;
            request.Method = "GET";
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate, sdch";
            request.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8,es;q=0.6,he;q=0.4";
            request.Headers[HttpRequestHeader.Authorization] = "Basic c2xpbTpzbGltZXIx";
            request.Referer = "http://192.168.0.1/userRpm/WlanStationRpm_5g.htm";

            request.Credentials = new NetworkCredential("admin", "admin");
            string html = string.Empty;
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        html = new StreamReader(stream).ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {

            }

            return html;
        }

        private  string extractMacsRawAray(string html)
        {
            var macsArray = string.Empty;
            if (!String.IsNullOrWhiteSpace(html))
            {
                macsArray = parseAfter(html, "var hostList = new Array(");
                if (!string.IsNullOrWhiteSpace(macsArray)) macsArray = parseBefore(macsArray, ");");
            }
            return macsArray;
        }

        private string parseAfter(string text, string indicator)
        {
            var parsed = string.Empty;
            var indicatorIndex = text.IndexOf(indicator);
            if (indicatorIndex > 0)
            {
                parsed = text.Substring(indicatorIndex + indicator.Length);
            }
            return parsed;
        }

        private string parseBefore(string text, string indicator)
        {
            var indicatorIndex = text.IndexOf(indicator);
            var parsed = string.Empty;
            if (indicatorIndex > 0)
            {
                parsed = text.Substring(1, indicatorIndex - indicator.Length);
            }
            return parsed;
        }

        private string parseMacFromString(string macAddressString)
        {
            if (string.IsNullOrWhiteSpace(macAddressString))
                return macAddressString;

            string[] stringsToRemove = { "\"", " ", ":", "-", @"\n",  };
            foreach (var stringToRemove in stringsToRemove)
            {
                macAddressString = macAddressString.Replace(stringToRemove, string.Empty);
            }
            return macAddressString.Trim().ToLower();
        }
    }
}
