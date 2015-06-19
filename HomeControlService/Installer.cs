using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using HomeControl;

namespace HomeControlService
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IHomeController>().ImplementedBy<HomeController>(),
                Component.For<HomeControlService>().LifestyleSingleton()
                );
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
        }
    }
}
