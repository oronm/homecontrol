using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net;

namespace HomeControl.Local.App
{
    // TPDP : fix deployment of all
    // TODO : Add authentication for honeyimhome
    // TODO : Add device information 
    // TODO : Add history
    // TODO : Add cache to hih controller
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
            try
            {
                //log.InfoFormat("Installing assemblies from here: {0}", Assembly.GetAssembly(typeof(LocalHomeControlService)).FullName);

                log.InfoFormat("Installing local");
                container.Install(
                    FromAssembly.Instance(Assembly.GetAssembly(typeof(LocalHomeControlService)))
                    //FromAssembly.Instance(Assembly.GetAssembly(typeof(Program)))
                    //FromAssembly.InDirectory(new AssemblyFilter(AssemblyDirectory))
                    );
                log.InfoFormat("Installing this");
                container.Install(FromAssembly.This());
                log.InfoFormat("Installing all assemblies from here: {0}", AssemblyDirectory);
                container.Install(FromAssembly.InDirectory(new AssemblyFilter(AssemblyDirectory)));
                //container.Install(FromAssembly.InDirectory(new AssemblyFilter(@".\")));
            }
            catch (Exception e)
            {
                log.Error("Failed loading app", e);
            }

            try
            {
                log.Info("installing fresh");
                container = new WindsorContainer();
                container.Install(FromAssembly.InDirectory(new AssemblyFilter(@".\")));
            }
            catch (Exception eex)
            {
                log.Error("eex", eex);
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1(null));
                Application.Run(container.Resolve<HomeControlApplicationContext>());
                //Application.Run(container.Resolve<Form1>());
            }
            catch (Exception ex)
            {
                log.Error("Error starting app", ex);
            }
        }

        static public string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;

                var uri = new UriBuilder(codeBase);

                var path = Uri.UnescapeDataString(uri.Path);

                return Path.GetDirectoryName(path);
            }
        }
    }
}
