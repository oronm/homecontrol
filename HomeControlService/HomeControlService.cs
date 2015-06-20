using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using HomeControl;
using log4net;

namespace HomeControlService
{
    public class HomeControlService
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IHomeController homeController;

        public HomeControlService(IHomeController homeController)
        {
            this.homeController = homeController;
        }

        internal void Start()
        {
            log.Info("Starting");
        }

        internal bool Stop()
        {
            log.Info("Stopping");
            return true;
        }
    }
}
