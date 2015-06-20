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
                Component.For<WifiDevicePresenceIdentifier>().LifeStyle.Singleton,
                Component.For<IPresnceIdentifier>().ImplementedBy<PresenceIdentifier>(),
                Component.For<IDevicePresenceFactory>().ImplementedBy<DevicePresenceFactory>().LifestyleSingleton()
                );
        }
    }
}
