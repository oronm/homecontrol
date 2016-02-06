using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using HomeControl.Cloud.Contracts;
using HomeControl.Cloud.Managers;
using log4net;
using Topshelf;
using Topshelf.ServiceConfigurators;

namespace HomeControl.Cloud.ManagersBasicHost
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
            var host = HostFactory.New(c =>
            {
                c.Service(new Action<ServiceConfigurator<WcfServiceWrapper<StateManager, IStateFeed>>>(s =>
                {
                    s.ConstructUsing( name => container.Resolve<WcfServiceWrapper<StateManager, IStateFeed>>());
                    s.WhenStarted(x => x.Start("Managers", "http://localhost:10000/Managers"));
                    s.WhenStopped(tc => tc.Stop());
                }));

                c.RunAsLocalSystem();
            });
            host.Run();
        }
    }
}
