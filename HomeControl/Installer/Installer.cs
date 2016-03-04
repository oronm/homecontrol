using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using HomeControl.PresenceManager;
using log4net;

namespace HomeControl.Installer
{
    public class Installer : IWindsorInstaller
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            log.Info("Installing HomeConrol");
            container.Register(
                Component.For<IPresenceManager>().ImplementedBy<HomeControl.PresenceManager.PresenceManager>()
                );
            log.Info("Installing HomeConrol Done");
        }
    }
}
