using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            GetPingSubtnets();
            //GetMacsUsingTPLINKWDR4300();
            //GetMacsUsingIPInfoNetsh();
            //GetMacsUsingIPInfo();
        }

        static void GetMacsUsingTPLINKWDR4300()
        {
            var scanner = new Helpers.Network.WiFi.TpLink_WDR430.MACScanner("192.168.0.1", "admin", "admin");
            foreach (var mac in scanner.GetConnectedMacs())
            {
                Console.WriteLine(mac);
            }
            Console.ReadLine();
        }

        static void GetMacsUsingIPInfo()
        {
            var info = IPInfo.GetIPInfoWithPings();

            foreach (var ip in info)
            {
                Console.WriteLine("{0}\t{1}", ip.IPAddress, ip.MacAddress);
            }

            Console.ReadLine();
        }
        static void GetMacsUsingIPInfoNetsh()
        {
            var info = IPInfo.GetIPInfoWithPings(IPInfoMethodType.Netsh);

            foreach (var ip in info)
            {
                Console.WriteLine("{0}\t{1}", ip.IPAddress, ip.MacAddress);
            }

            Console.ReadLine();
        }
        static void GetPingSubtnets()
        {
            var info = IPInfo.BroadCastPing();
            //var info = IPInfo.BroadCastPing("192.168.0", "10.0.0");

            foreach (var ip in info)
            {
                Console.WriteLine("{0}", ip);
            }

            Console.ReadLine();
        }

    }
}
