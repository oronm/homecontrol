using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using HomeControl.Common;
using log4net;

namespace HomeControl
{
    public class HomeController : IHomeController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IDictionary<string, PersonState> state = null;
        private IPresnceIdentifier identifier;
        private ConcurrentQueue<Action> presenceActions = new ConcurrentQueue<Action>();


        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived;
        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft; 
    

        public HomeController(IPresnceIdentifier identifier)
        {
            this.identifier = identifier;
            identifier.PersonArrived += identifier_PersonArrived;
            identifier.PersonLeft += identifier_PersonLeft;

            identifier.registerPerson(new PersonRegistration()
            {
                personName = "Galia",
                devicesDetails = new IDeviceDetails[] { 
                    new WifiDeviceDetails() { DeviceId = "70-3E-AC-4C-AC-54", DeviceName = "Galia's iPhone" } 
                }
            });
            identifier.registerPerson(new PersonRegistration()
            {
                personName = "Oron",
                devicesDetails = new IDeviceDetails[] { 
                    new WifiDeviceDetails() { DeviceId = "D4-F4-6F-23-93-09", DeviceName = "Oron's iPhone" } 
                }
            });


            state = identifier.getState();
            Helper.StartRepetativeTask(HandleNewEvents, TimeSpan.FromSeconds(10));
        }

        private void HandleNewEvents()
        {
            Action eventAction;
            while (presenceActions.TryDequeue(out eventAction))
            {
                eventAction();
            }
        }

        private void identifier_PersonLeft(object sender, string e)
        {
            if (e == "Oron") presenceActions.Enqueue(NotifyOronLeftHome);
            else if (e == "Galia") presenceActions.Enqueue(NotifyGaliaLeftHome);
        }

        private void identifier_PersonArrived(object sender, string e)
        {
            if (e == "Oron") presenceActions.Enqueue(NotifyOronArrivedHome);
            else if (e == "Galia") presenceActions.Enqueue(NotifyGaliaArrivedHome);
        }

        private void NotifyOronArrivedHome()
        {
            log.Info("Oron is Home!");
            Helper.StartProcess(@"E:\Programs\OronIsHome.bat");
            NotifyPersonArrived("Oron");
        }

        private void NotifyPersonArrived(string p)
        {
            Notify(this.OnPersonArrived, p);
        }

        private void NotifyPersonLeft(string p)
        {
            Notify(this.OnPersonLeft, p);
        }
        
        private void Notify(EventHandler<PersonPresenceChangedEventArgs> eventHandler, string p)
        {
            var tmp = eventHandler;
            if (tmp != null) tmp(this, new PersonPresenceChangedEventArgs() { Name = p, ChangeTimeUtc = DateTime.UtcNow });
        }
        
        private void NotifyOronLeftHome()
        {
            Helper.StartProcess(@"E:\Programs\OronIsNotHome.bat");
            log.Info("oron left home!");
            NotifyPersonLeft("Oron");
        }

        private void NotifyGaliaArrivedHome()
        {
            log.Info("galia is home!");
            PersonState oronState;
            bool oronInState = state.TryGetValue("Oron", out oronState);
            if ( !oronInState || !oronState.IsPresent())
            {
                Helper.StartProcess(@"E:\Programs\GaliaIsHome.bat");
            }
            NotifyPersonArrived("Galia");
        }
        private void NotifyGaliaLeftHome()
        {
            log.Info("Galia left home!");
            Helper.StartProcess(@"E:\Programs\GaliaIsNotHome.bat");
            NotifyPersonLeft("Galia");
        }
    }
}
