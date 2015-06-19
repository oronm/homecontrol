using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using HomeControl;

namespace HomeControlService
{
    public class HomeControlService
    {
        private ILogger logger;
        private IHomeController homeController;

        public HomeControlService(ILogger logger, IHomeController homeController)
        {
            this.logger = logger;
            this.homeController = homeController;
        }

        internal void Start()
        {
            logger.Info("Starting");
        }

        internal bool Stop()
        {
            logger.Info("Stopping");
            return true;
        }
    }
}
