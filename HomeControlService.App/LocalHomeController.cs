using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using HomeControl.Local.Contracts;
using log4net;

namespace HomeControl.Local.App
{
    public class LocalHomeController : IHomeController
    {
        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived;
        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft;

        protected readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected ILocalHomeControlService homeController;

        public LocalHomeController(ILocalHomeControlService homeController)
        {
            this.homeController = homeController;
            this.homeController.OnPersonArrived += homeController_OnPersonArrived;
            this.homeController.OnPersonLeft += homeController_OnPersonLeft;
        }

        private void homeController_OnPersonLeft(object sender, PersonPresenceChangedEventArgs e)
        {
            HandlePersonLeft(sender, e);
            Notify(this.OnPersonLeft, e);
        }

        private void homeController_OnPersonArrived(object sender, PersonPresenceChangedEventArgs e)
        {
            HandlePersonArrived(sender, e);
            Notify(this.OnPersonArrived, e);
        }

        protected virtual void HandlePersonArrived(object sender, PersonPresenceChangedEventArgs e)
        {
        }

        protected virtual void HandlePersonLeft(object sender, PersonPresenceChangedEventArgs e)
        {
        }

        void Notify(EventHandler<PersonPresenceChangedEventArgs> handler, PersonPresenceChangedEventArgs e)
        {
            var tmp = handler;
            if (tmp != null) tmp(this, e);
        }
    }

    public class MoradHomeController : LocalHomeController
    {
        public MoradHomeController(ILocalHomeControlService homeController)
            : base(homeController)
        {

        }

        protected override void HandlePersonArrived(object sender, PersonPresenceChangedEventArgs e)
        {
            if (e.Name == "Oron") NotifyOronArrivedHome();
            else if (e.Name == "Galia") NotifyGaliaArrivedHome();
        }
        protected override void HandlePersonLeft(object sender, PersonPresenceChangedEventArgs e)
        {
            if (e.Name == "Oron") NotifyOronLeftHome();
            else if (e.Name == "Galia") NotifyGaliaLeftHome();
        }

        private void NotifyOronLeftHome()
        {
            Helper.StartProcess(@"E:\Programs\OronIsNotHome.bat");
            log.Info("oron left home!");
        }
        private void NotifyOronArrivedHome()
        {
            log.Info("Oron is Home!");
            Helper.StartProcess(@"E:\Programs\OronIsHome.bat");
        }
        private void NotifyGaliaArrivedHome()
        {
            IEnumerable<PersonState> state = this.homeController.GetState();
            log.Info("galia is home!");
            PersonState oronState = state.SingleOrDefault(st => st.name == "Oron");
            if (oronState == null || !oronState.IsPresent)
            {
                Helper.StartProcess(@"E:\Programs\GaliaIsHome.bat");
            }
        }
        private void NotifyGaliaLeftHome()
        {
            log.Info("Galia left home!");
            Helper.StartProcess(@"E:\Programs\GaliaIsNotHome.bat");
        }

    }
}
