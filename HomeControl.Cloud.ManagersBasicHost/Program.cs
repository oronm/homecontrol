using System;
using System.Collections.Generic;
using System.Configuration;
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
    class Services
    {
        public WcfServiceWrapper<StateManager, IStateReport> ReportSVC;
        public WcfServiceWrapper<StateManager, IStateFeed> FeedSVC;
        public Services(WcfServiceWrapper<StateManager, IStateReport> reportSVC, WcfServiceWrapper<StateManager, IStateFeed> feedSVC)
        {
            ReportSVC = reportSVC;
            FeedSVC = feedSVC;
        }
    }

    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            log.Debug("Debug Message..");
            log.Warn("Warning Message..");
            log.Error("Error Message..");
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
                c.Service(new Action<ServiceConfigurator<Services>>(s =>
                {
                    s.ConstructUsing(name => container.Resolve<Services>());
                    s.WhenStarted(x => { 
                        x.ReportSVC.Start("Report", ConfigurationManager.AppSettings["SVC.Report.Endpoint"]); 
                        x.FeedSVC.Start("Managers", ConfigurationManager.AppSettings["SVC.Managers.Endpoint"]); 
                    });
                    s.WhenStopped(x => { x.ReportSVC.Stop(); x.FeedSVC.Stop(); });
                }));

                c.RunAsLocalSystem();
            });
            //var host2 = HostFactory.New(c =>
            //{
            //    c.Service(new Action<ServiceConfigurator<WcfServiceWrapper<StateManager, IStateReport>>>(s =>
            //    {
            //        s.ConstructUsing(name => container.Resolve<WcfServiceWrapper<StateManager, IStateReport>>());
            //        s.WhenStarted(x => x.Start("Report", "http://localhost:10001/Report"));
            //        s.WhenStopped(tc => tc.Stop());
            //    }));

            //    c.RunAsLocalSystem();
            //});

            host.Run();
            //host2.Run();
        }
    }
}
