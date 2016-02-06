using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using HomeControl.Cloud.Contracts;
using HomeControl.Cloud.Managers;

namespace HomeControl.Cloud.ManagersBasicHost
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IStateFeed>().ImplementedBy<StateManager>().LifestyleSingleton(),
                Component.For<WcfServiceWrapper<StateManager, IStateFeed>>()
                );
        }
    }
}
