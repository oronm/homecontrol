using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Helpers.Network.WiFi.TpLink_WDR430
{
    public class MACScanner
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _routerIP;
        private readonly string _adminUsername;
        private readonly string _adminPassword;

        public MACScanner(string routerIP, string adminUsername, string adminPassword)
        {
            if (string.IsNullOrWhiteSpace(routerIP) ||
                string.IsNullOrWhiteSpace(adminUsername) ||
                string.IsNullOrWhiteSpace(adminPassword))
                throw new ArgumentNullException();

            System.Net.IPAddress.Parse(routerIP);

            this._routerIP = routerIP;
            this._adminPassword = adminPassword;
            this._adminUsername = adminUsername;
        }
        
        public IEnumerable<string> GetConnectedMacs()
        {
            return getConnectedMacs(string.Format("http://{0}/userRpm/WlanStationRpm_5g.htm?Page=1",_routerIP)).Union(
             getConnectedMacs(string.Format("http://{0}/userRpm/WlanStationRpm.htm?Page=1",_routerIP)));
        }

        private IEnumerable<string> getConnectedMacs(string urlAddress)
        {
            IEnumerable<string> macs = new string[] {};

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
            request.Referer = string.Format("http://{0}/userRpm/WlanStationRpm_5g.htm",_routerIP);

            request.Credentials = new NetworkCredential(_adminUsername, _adminPassword);
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
                log.Error(string.Format("Error getting response from {0}",urlAddress), e);
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
