using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

/// <summary>
/// This class allows you to retrieve the IP Address and Host Name for a specific machine on the local network when you only know it's MAC Address.
/// http://stackoverflow.com/questions/10360933/find-interfaces-and-ip-addresses-arp-in-c
/// </summary>
public class IPInfo
{
    public IPInfo(string macAddress, string ipAddress)
    {
        this.MacAddress = macAddress;
        this.IPAddress = ipAddress;
    }

    public string MacAddress { get; private set; }
    public string IPAddress { get; private set; }

    private string _HostName = string.Empty;
    public string HostName
    {
        get
        {
            if (string.IsNullOrEmpty(this._HostName))
            {
                try
                {
                    // Retrieve the "Host Name" for this IP Address. This is the "Name" of the machine.
                    this._HostName = Dns.GetHostEntry(this.IPAddress).HostName;
                }
                catch
                {
                    this._HostName = string.Empty;
                }
            }
            return this._HostName;
        }
    }


    #region "Static Methods"

    /// <summary>
    /// Retrieves the IPInfo for the machine on the local network with the specified MAC Address.
    /// </summary>
    /// <param name="macAddress">The MAC Address of the IPInfo to retrieve.</param>
    /// <returns></returns>
    public static IPInfo GetIPInfo(string macAddress)
    {
        var ipinfo = (from ip in IPInfo.GetIPInfo()
                      where ip.MacAddress.ToLowerInvariant() == macAddress.ToLowerInvariant()
                      select ip).FirstOrDefault();

        return ipinfo;
    }
    public static List<IPInfo> GetIPInfoWithPings(IEnumerable<string> macAddresses = null)
    {
        return GetIPInfoWithPings(IPInfoMethodType.ARP, macAddresses);
    }
    public static List<IPInfo> GetIPInfoWithPings(IPInfoMethodType methodType, IEnumerable<string> macAddresses = null)
    {
        IEnumerable<IPInfo> ips = GetIPInfo(methodType);
        if (macAddresses != null)
            ips = filterMacAddresses(ips, macAddresses.Select(macaddress => parseMacFromString(macaddress)).ToArray());
        Parallel.ForEach(ips, (ip) => { ip.Ping(); });
        return GetIPInfo(methodType);
    }
    /// <summary>
    /// Retrieves the IPInfo for All machines on the local network.
    /// </summary>
    /// <returns></returns>
    public static List<IPInfo> GetIPInfo(bool dynamicOnly = true)
    {
        return GetIPInfo(IPInfoMethodType.ARP, dynamicOnly);
    }
    public static List<IPInfo> GetIPInfo(IPInfoMethodType methodType, bool dynamicOnly = true)
    {
        try
        {
            var list = new List<IPInfo>();

            Func<string> resultMethod;
            Func<string, bool> dynamicDescriminator;

            switch (methodType)
            {
                case IPInfoMethodType.Netsh:
                    var filters = new string[] { "permanent", "incomplete" };
                    dynamicDescriminator = (desc) => !filters.Contains(desc.ToLower()) && !desc.Contains("---");
                    resultMethod = GetNetshResult;
                    break;
                case IPInfoMethodType.ARP:
                default:
                    dynamicDescriminator = (desc) => desc.ToLower() == "dynamic";
                    resultMethod = GetARPResult;
                    break;
            }


            foreach (var arp in resultMethod().Split(new char[] { '\n', '\r' }))
            {
                // Parse out all the MAC / IP Address combinations
                if (!string.IsNullOrEmpty(arp))
                {
                    var pieces = (from piece in arp.Split(new char[] { ' ', '\t' })
                                  where !string.IsNullOrEmpty(piece)
                                  select piece).ToArray();

                    if (pieces.Length == 3 &&
                        (!dynamicOnly || (dynamicOnly && dynamicDescriminator(pieces[2]))) &&
                        isRealMacAddress(pieces[1]))
                    {
                        list.Add(new IPInfo(pieces[1], pieces[0]));
                    }
                }
            }

            // Return list of IPInfo objects containing MAC / IP Address combinations
            return list;
        }
        catch (Exception ex)
        {
            throw new Exception("IPInfo: Error Parsing 'arp -a' results", ex);
        }
    }


    private static IEnumerable<IPInfo> filterMacAddresses(IEnumerable<IPInfo> ips, IEnumerable<string> macAddresses)
    {
        var set = new HashSet<string>(macAddresses);
        return ips.Where(ipinfo => set.Contains(parseMacFromString(ipinfo.MacAddress)));
    }
    private static string parseMacFromString(string macAddressString)
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


    /// <param name="times">Number of echo requests, 1-10</param>
    public void Ping(int times=2)
    {
        if (times < 1 || times > 10)
        {
            throw new ArgumentException("times");
        }

        try
        {
            Process.Start(new ProcessStartInfo("ping", string.Format("-n {0} {1}", times, IPAddress))
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            }).WaitForExit(5000);
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("IPInfo: Error pinging {0}", this.IPAddress), ex);
        }
    }

    private static bool isRealMacAddress(string mac)
    {
        mac = mac ?? "";
        mac = mac.Trim().ToLower();
        var filters = new string[] { "unreachable" };
        return (!string.IsNullOrWhiteSpace(mac) && !filters.Contains(mac));
    }

    /// <summary>
    /// This runs the "arp" utility in Windows to retrieve all the MAC / IP Address entries.
    /// </summary>
    /// <returns></returns>
    private static string GetARPResult()
    {
        return getCmdResult("arp", "-a");
    }
    private static string GetNetshResult()
    {
        return getCmdResult("netsh", "interface ip show neighbors");
    }

    private static string getCmdResult(string command, string args)
    {
        Process p = null;
        string output = string.Empty;

        try
        {
            p = Process.Start(new ProcessStartInfo(command, args)
            {
                
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            });

            output = p.StandardOutput.ReadToEnd();

            p.Close();
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("IPInfo: Error Retrieving '{0} {1}' Results", command, args), ex);
        }
        finally
        {
            if (p != null)
            {
                p.Close();
            }
        }

        return output;
    }


    public static string[] BroadCastPing(string subNet)
    {
        return BroadCastPing(new string[] { subNet });
    }
    public static string[] BroadCastPing(params string[] subNets)
    {
        if (subNets == null || subNets.Length == 0)
        {
            subNets = detectSubnets();
        }
        if (subNets == null || subNets.Length == 0)
        {
            return new string[]{};
        }

        var ips = generateIps(subNets);

        Parallel.ForEach(ips, new ParallelOptions() { MaxDegreeOfParallelism=5 }, ip => { ip.Ping(); });

        return ips.Select(ipinfo => ipinfo.IPAddress).ToArray();
    }

    private static IPInfo[] generateIps(string[] subNets)
    {
        int minRange = 1;
        int maxRange = 254;
        var subNetsIps = new List<IPInfo>((maxRange - minRange + 1)*subNets.Length);
        for (int subnetIndex = 0; subnetIndex < subNets.Length; subnetIndex++)
        {
            for (int i = minRange; i <= maxRange; i++)
            {
                subNetsIps.Add(new IPInfo("", string.Format("{0}.{1}", subNets[subnetIndex], i)));
            }
        }

        return subNetsIps.ToArray();
    }

    private static string[] detectSubnets()
    {
        var ips = GetIPInfo();
        var subnets =
            from ip in ips
            group ip by extractSubnet(ip) into subnetsgroup
            select subnetsgroup.Key;

        return subnets.ToArray();
    }

    private static string extractSubnet(IPInfo ip)
    {
        var parts = ip.IPAddress.Split('.');
        if (parts.Length < 3) return string.Empty;

        return string.Join(".", parts.Take(3));
    }


    #endregion
}

public enum IPInfoMethodType
{
    ARP,
    Netsh
}

