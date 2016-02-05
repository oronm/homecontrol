using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using HomeControl;
using HomeControl.Local.Contracts;
using log4net;

namespace HomeControl.Local
{
    public class LocalHomeControlService : ILocalHomeControlService
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IHomeController homeController;

        public event EventHandler<Contracts.PersonPresenceChangedEventArgs> OnPersonArrived;
        public event EventHandler<Contracts.PersonPresenceChangedEventArgs> OnPersonLeft;

        public LocalHomeControlService(IHomeController homeController)
        {
            this.homeController = homeController;
            this.homeController.OnPersonArrived += homeController_OnPersonArrived;
            this.homeController.OnPersonLeft += homeController_OnPersonLeft;
        }


        private void homeController_OnPersonLeft(object sender, HomeControl.PersonPresenceChangedEventArgs e)
        {
            Notify(this.OnPersonLeft, e);
        }

        private void homeController_OnPersonArrived(object sender, HomeControl.PersonPresenceChangedEventArgs e)
        {
            Notify(this.OnPersonArrived, e);
        }

        void Notify(EventHandler<Contracts.PersonPresenceChangedEventArgs> handler, HomeControl.PersonPresenceChangedEventArgs e)
        {
            var tmp = handler;
            if (tmp != null) tmp(this, new Contracts.PersonPresenceChangedEventArgs(e));
        }


        public void Start()
        {
            log.Info("Starting");
            homeController.OnPersonArrived += homeController_OnPersonArrived;
            homeController.OnPersonLeft += homeController_OnPersonLeft;
        }

        public bool Stop()
        {
            log.Info("Stopping");
            homeController.OnPersonArrived -= homeController_OnPersonArrived;
            homeController.OnPersonLeft -= homeController_OnPersonLeft;
            
            return true;
        }


        public IEnumerable<PersonState> GetState()
        {
            return this.homeController.GetState().Select(contollerState => new PersonState(){ 
                name = contollerState.name,
                lastSeen = contollerState.lastSeen,
                lastLeft = contollerState.lastLeft,
                IsPresent = contollerState.IsPresent()
            });
        }
    }
}
