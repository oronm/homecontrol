using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using HomeControl.Detection;
using WifiDeviceIdentifier;

namespace HomeControl.PresenceManager.Installer
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<WifiDeviceSensor>().LifeStyle.Singleton,
                Component.For<ISensorFactory>().ImplementedBy<SingletonsSensorFactory>().LifestyleSingleton()
                );
        }
    }
}
