using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using HomeControl.Local.Contracts;
using log4net;

namespace HomeControl.Local.App
{
    public class Installer : IWindsorInstaller
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            log.Info("Installing HomeConrol.Local.App");
            container.Register(
                Component.For<ILocalHomeControlService>().ImplementedBy<LocalHomeControlService>().LifestyleSingleton(),
                Component.For<IHomeController>().ImplementedBy<MoradHomeController>().LifestyleSingleton(),
                Component.For<HomeControlApplicationContext>(),
                Component.For<Form1>()
                );
            log.Info("Installing HomeConrol.Local.App Done");
        }
    }
}
