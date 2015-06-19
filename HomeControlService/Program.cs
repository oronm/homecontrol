using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Logging;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Topshelf;
using Topshelf.ServiceConfigurators;

namespace HomeControlService
{
    class Program
    {
        static void Main(string[] args)
        {
            //runService();
            string urlAddress = "http://192.168.0.1/userRpm/WlanStationRpm_5g.htm?Page=1";
            //string urlAddress = "http://192.168.0.1/userRpm/WlanStationRpm_5g.htm";
        
            string myParameters = "";

            HttpWebRequest request = HttpWebRequest.Create(urlAddress) as HttpWebRequest;
            request.Method = "GET";
            //request.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate, sdch";
            request.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8,es;q=0.6,he;q=0.4";
            //request.Headers[HttpRequestHeader.Authorization] = "Basic c2xpbTpzbGltZXIx";
            request.Referer = "http://192.168.0.1/userRpm/WlanStationRpm_5g.htm";
            //request.Referer = "http://192.168.0.1/userRpm/MenuRpm.htm";

        
            //request.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.124 Safari/537.36";
            

//            Cache-Control:max-age=0
//Connection:keep-alive
//Host:192.168.0.1

            request.Credentials = new NetworkCredential("slim", "slimer1");
            string html = string.Empty;
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    html = new StreamReader(stream).ReadToEnd();
                }
            }
            if (!String.IsNullOrWhiteSpace(html))
            {
                string indicator = "var hostList = new Array(";
                var indicatorIndex = html.IndexOf(indicator);
                if (indicatorIndex > 0)
                {
                    var array = html.Substring(indicatorIndex + indicator.Length);
                    indicator = ");";
                    indicatorIndex = array.IndexOf(indicator);
                    if (indicatorIndex > 0)
                    {
                        array = array.Substring(1, indicatorIndex-indicator.Length);
                    }
                }
            }

            //using (WebClient wc = new WebClient())
            //{
            //    //wc.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            //    wc.Credentials = new NetworkCredential("slim", "slimer1");
            //    string HtmlResult = wc.UploadString(urlAddress, "GET", myParameters);
            //}
            Console.ReadLine();
        }

        private static void runService()
        {
            var container = new WindsorContainer();
            container.Install(FromAssembly.InThisApplication());
            var host = HostFactory.New(x =>
            {
                x.Service(new Action<ServiceConfigurator<HomeControlService>>(s =>
                {
                    s.ConstructUsing(name => container.Resolve<HomeControlService>());
                    s.WhenStarted(tc =>
                    {
                        tc.Start();
                    });
                    s.WhenStopped(tc => tc.Stop());
                }));

                x.RunAsLocalSystem();
            });

            host.Run();
        }
    }
}

