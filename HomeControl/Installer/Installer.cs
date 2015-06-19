using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using HomeControl.Common;
using WifiDeviceIdentifier;

namespace HomeControl.Installer
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IDevicePresenceIdentifier>().ImplementedBy<WifiDevicePresenceIdentifier>().Named(typeof(WifiDevicePresenceIdentifier).Name).LifeStyle.Singleton
                );
        }
    }
}
