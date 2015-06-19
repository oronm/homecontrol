using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Console.ReadLine();
        }
    }
}

