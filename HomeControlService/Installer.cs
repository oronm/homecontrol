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
using log4net;

namespace HomeControl.Local
{
    public class Installer : IWindsorInstaller
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            log.Info("Installing HomeConrol.Local - LocalHomeControlService");
            container.Register(
                Component.For<IHomeController>().ImplementedBy<HomeController>(),
                Component.For<IStateReportersFactory>().ImplementedBy<StateReportersFactory>(),
                Component.For<IResidentsRepository>().UsingFactoryMethod( (c) => new LocalFileResidentsRepository(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["connectionString"])
                    ))
                );
            log.Info("Installing HomeConrol.Local - LocalHomeControlService - Done");
        }
    }
}
