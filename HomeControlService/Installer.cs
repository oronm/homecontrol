using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                Component.For<IResidentsRepository>().UsingFactoryMethod( (c) => new LocalFileResidentsRepository(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["connectionString"])
                    ))
                );
        }
    }
}
