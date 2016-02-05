using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using HomeControl.Local.Contracts;

namespace HomeControl.Local.App
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILocalHomeControlService>().ImplementedBy<LocalHomeControlService>().LifestyleSingleton(),
                Component.For<IHomeController>().ImplementedBy<MoradHomeController>().LifestyleSingleton(),
                Component.For<HomeControlApplicationContext>(),
                Component.For<Form1>()
                );
        }
    }
}
