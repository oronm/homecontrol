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

        public void Start()
        {
            log.Info("Starting");
        }

        public bool Stop()
        {
            log.Info("Stopping");
            return true;
        }
    }
}
