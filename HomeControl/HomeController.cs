using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace BluetoothWatcherService
{
    public class HomeController : BluetoothListenerService.IHomeController
    {
        private IDictionary<string, PersonState> state = null;

        public HomeController(IPresnceIdentifier identifier)
        {
            identifier.PersonArrived += identifier_PersonArrived;
            identifier.PersonLeft += identifier_PersonLeft;

            identifier.registerPerson("oron");
            identifier.registerPerson("galia");

            state = identifier.getState();
        }

        void identifier_PersonLeft(object sender, string e)
        {
            this.state = (sender as IPresnceIdentifier).getState();
            if (e == "oron") NotifyOronLeftHome();
            else if (e == "galia") NotifyGaliaLeftHome();
        }

        void identifier_PersonArrived(object sender, string e)
        {
            this.state = (sender as IPresnceIdentifier).getState();
            if (e == "oron") NotifyOronArrivedHome();
            else if (e == "galia") NotifyGaliaArrivedHome();
        }

        public void NotifyOronArrivedHome()
        {
            Console.WriteLine("Oron is Home!");
            Helper.StartProcess(@"E:\Programs\OronIsHome.bat");
        }
        public void NotifyOronLeftHome()
        {
            Helper.StartProcess(@"E:\Programs\OronIsNotHome.bat");
            Console.WriteLine("oron left home!");

        }
        public void NotifyGaliaArrivedHome()
        {
            Console.WriteLine("galia is home!");
            PersonState oronState;
            if (state.TryGetValue("oron", out oronState) && !oronState.IsPresent())
            {
                Helper.StartProcess(@"E:\Programs\GaliaIsHome.bat");
            }
        }
        public void NotifyGaliaLeftHome()
        {
            Console.WriteLine("Galia left home!");
            Helper.StartProcess(@"E:\Programs\GaliaIsNotHome.bat");
        }
    }
}
