using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using log4net;
using HomeControl.PresenceManager;
using HomeControl.Detection;

namespace HomeControl
{
    public class HomeController : IHomeController
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected IDictionary<string, PersonState> state = null;
        protected IPresenceManager identifier;
        protected IResidentsRepository residentsRepository;
        protected ConcurrentQueue<Action> presenceActions = new ConcurrentQueue<Action>();

        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonArrived;
        public event EventHandler<PersonPresenceChangedEventArgs> OnPersonLeft; 

        public HomeController(IPresenceManager identifier, IResidentsRepository residentsRepository)
        {
            this.identifier = identifier;
            this.residentsRepository = residentsRepository;
            identifier.PersonArrived += identifier_PersonArrived;
            identifier.PersonLeft += identifier_PersonLeft;

            registerResidents();


            state = identifier.GetState();
            Helper.StartRepetativeTask(HandleNewEvents, TimeSpan.FromSeconds(10));
        }

        public IEnumerable<PersonState> GetState()
        {
            return identifier.GetState().Select((kvp) => kvp.Value).ToArray();
        }

        protected virtual void registerResidents()
        {
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
                        PersonName = residentDevices.Key,
                        Detectables = residentDevices.Select(device => createDeviceDetails(device))
                    };

                log.Info("Registering Residents");
                foreach (var resident in residents)
                {
                    string residentDetails = string.Format("{0} with {1} device(s)", resident.PersonName, resident.Detectables == null ? 0 : resident.Detectables.Count());
                    try
                    {

                        identifier.RegisterPerson(resident);
                        log.InfoFormat("Registered resident {0}", residentDetails);
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Error registering resident {0}", residentDetails), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to register residents, error loading identifying devices", ex);
                throw;
            }
        }
        private IDetectable createDeviceDetails(IdentifyingDevice device)
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
            state = identifier.GetState();
            Action eventAction;
            while (presenceActions.TryDequeue(out eventAction))
            {
                eventAction();
            }
        }

        private void identifier_PersonLeft(object sender, string e)
        {
            presenceActions.Enqueue(() => NotifyPersonLeft(e));
        }

        private void identifier_PersonArrived(object sender, string e)
        {
            presenceActions.Enqueue(() => NotifyPersonArrived(e));
        }

        protected virtual void NotifyPersonArrived(string p)
        {
            Notify(this.OnPersonArrived, p);
        }

        protected virtual void NotifyPersonLeft(string p)
        {
            Notify(this.OnPersonLeft, p);
        }
        
        protected void Notify(EventHandler<PersonPresenceChangedEventArgs> eventHandler, string p)
        {
            var tmp = eventHandler;
            if (tmp != null) tmp(this, new PersonPresenceChangedEventArgs() { Name = p, ChangeTimeUtc = DateTime.UtcNow });
        }

    }
}
