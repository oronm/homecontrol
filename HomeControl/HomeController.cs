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
        private IResidentsRepository residentsRepository;
        private ConcurrentQueue<Action> presenceActions = new ConcurrentQueue<Action>();


        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived;
        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft; 
    

        public HomeController(IPresnceIdentifier identifier, IResidentsRepository residentsRepository)
        {
            this.identifier = identifier;
            this.residentsRepository = residentsRepository;
            identifier.PersonArrived += identifier_PersonArrived;
            identifier.PersonLeft += identifier_PersonLeft;

            registerResidents();


            state = identifier.getState();
            Helper.StartRepetativeTask(HandleNewEvents, TimeSpan.FromSeconds(10));
        }

        private void registerResidents()
        {
            //identifier.registerPerson(new PersonRegistration()
            //{
            //    personName = "Galia",
            //    devicesDetails = new IDeviceDetails[] { new WifiDeviceDetails("Galia's iPhone", "70-3E-AC-4C-AC-54") }

            //});
            //identifier.registerPerson(new PersonRegistration()
            //{
            //    personName = "Oron",
            //    devicesDetails = new IDeviceDetails[] { new WifiDeviceDetails("Oron's iPhone", "D4-F4-6F-23-93-09") }

            //});

            try
            {
                var identifyingDevices = residentsRepository.GetIdentifyingDevices();
                if (identifyingDevices == null || identifyingDevices.Count() == 0)
                {
                    log.Info("No residents registered, identifying devices list is empty");
                    return;
                }

                // Group devices by resident
                var residents =
                    from identifyingDevice in identifyingDevices
                    group identifyingDevice by identifyingDevice.Owner into residentDevices
                    select new PersonRegistration
                    {
                        personName = residentDevices.Key,
                        devicesDetails = residentDevices.Select(device => createDeviceDetails(device))
                    };

                log.Info("Registering Residents");
                foreach (var resident in residents)
                {
                    string residentDetails = string.Format("{0} with {1} device(s)", resident.personName, resident.devicesDetails == null ? 0 : resident.devicesDetails.Count());
                    try
                    {
                        
                        identifier.registerPerson(resident);
                        log.InfoFormat("Registered resident {0}", residentDetails);
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Error registering resident {0}",residentDetails),ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to register residents, error loading identifying devices", ex);
                throw;
            }
        }

        private IDeviceDetails createDeviceDetails(IdentifyingDevice device)
        {
            if (device.IdentificationMethod != IdentificationMethodType.Wifi)
            {
                log.ErrorFormat("Encountered unsupported identifcation method {0} for resident {1}, device name={2} id={3}", device.IdentificationMethod, device.Owner, device.DeviceName, device.DeviceId);
                return null;
            }

            return new WifiDeviceDetails(device.DeviceName, device.DeviceId);
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
