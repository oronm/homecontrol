using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net;

namespace HomeControlService.App
{
    // TODO: Separate the srv to be system service and not user app
    // TODO: Web server the holds the state
    // TODO: Web app that shows the state

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        static void Main()
        {
            log.Info("Starting");
            var container = new WindsorContainer();
            container.Install(FromAssembly.InDirectory(new AssemblyFilter(@".\")));
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1(null));
            Application.Run(container.Resolve<HomeControlApplicationContext>());
            //Application.Run(container.Resolve<Form1>());
        }
    }
}
