using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net;
using Topshelf;
using Topshelf.ServiceConfigurators;

namespace HomeControlService
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            log.Info("Starting..");
            runService();
            Console.ReadLine();
        }

        private static void runService()
        {
            var container = new WindsorContainer();
            container.Install(FromAssembly.InDirectory(new AssemblyFilter(@".\")));
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

