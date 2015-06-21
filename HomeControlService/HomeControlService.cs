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

        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived;
        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft;

        public HomeControlService(IHomeController homeController)
        {
            this.homeController = homeController;
            homeController.OnPersonArrived += homeController_OnPersonArrived;
            homeController.OnPersonLeft += homeController_OnPersonLeft;
        }

        void homeController_OnPersonLeft(object sender, HomeControl.PersonPresenceChangedEventArgs e)
        {
            Notify(this.OnPersonLeft, e);
        }

        void homeController_OnPersonArrived(object sender, HomeControl.PersonPresenceChangedEventArgs e)
        {
            Notify(this.OnPersonArrived, e);
        }

        void Notify(EventHandler<PersonPresenceChangedEventArgs> handler, HomeControl.PersonPresenceChangedEventArgs e)
        {
            var tmp = handler;
            if (tmp != null) tmp(this, new PersonPresenceChangedEventArgs(e));
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
