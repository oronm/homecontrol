using System;
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

        public HomeController(IPresnceIdentifier identifier)
        {
            this.identifier = identifier;
            identifier.PersonArrived += identifier_PersonArrived;
            identifier.PersonLeft += identifier_PersonLeft;

            identifier.registerPerson(new PersonRegistration()
            {
                personName = "Oron",
                devicesDetails = new IDeviceDetails[] { 
                    new WifiDeviceDetails() { DeviceId = "D4-F4-6F-23-93-09", DeviceName = "Oron's iPhone" } 
                }
            });
            identifier.registerPerson(new PersonRegistration()
            {
                personName = "Galia",
                devicesDetails = new IDeviceDetails[] { 
                    new WifiDeviceDetails() { DeviceId = "70-3E-AC-4C-AC-54", DeviceName = "Galia's iPhone" } 
                }
            });

            state = identifier.getState();
        }

        void identifier_PersonLeft(object sender, string e)
        {
            this.state = (sender as IPresnceIdentifier).getState();
            if (e == "Oron") NotifyOronLeftHome();
            else if (e == "Galia") NotifyGaliaLeftHome();
        }

        void identifier_PersonArrived(object sender, string e)
        {
            this.state = (sender as IPresnceIdentifier).getState();
            if (e == "Oron") NotifyOronArrivedHome();
            else if (e == "Galia") NotifyGaliaArrivedHome();
        }

        public void NotifyOronArrivedHome()
        {
            log.Info("Oron is Home!");
            Helper.StartProcess(@"E:\Programs\OronIsHome.bat");
        }
        public void NotifyOronLeftHome()
        {
            Helper.StartProcess(@"E:\Programs\OronIsNotHome.bat");
            log.Info("oron left home!");

        }
        public void NotifyGaliaArrivedHome()
        {
            log.Info("galia is home!");
            PersonState oronState;
            if (state.TryGetValue("Oron", out oronState) && !oronState.IsPresent())
            {
                Helper.StartProcess(@"E:\Programs\GaliaIsHome.bat");
            }
        }
        public void NotifyGaliaLeftHome()
        {
            log.Info("Galia left home!");
            Helper.StartProcess(@"E:\Programs\GaliaIsNotHome.bat");
        }
    }
}
