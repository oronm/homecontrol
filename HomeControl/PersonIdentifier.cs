using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Common;
using Castle.Facilities.TypedFactory;
using log4net;

namespace HomeControl
{
    public class PersonStateConfiguration
    {
        //public TimeSpan MaximumAllowedDisconnection = TimeSpan.FromSeconds(20);
        public TimeSpan MaximumAllowedDisconnection = TimeSpan.FromMinutes(5);
        private CancellationTokenSource presenceTimeoutCancellation;

        public CancellationTokenSource resetTimeoutCancellation()
        {
            cancelPresenceTimeout();
            presenceTimeoutCancellation = new CancellationTokenSource();
            return presenceTimeoutCancellation;
        }
        public void cancelPresenceTimeout()
        {
            if (presenceTimeoutCancellation != null) presenceTimeoutCancellation.Cancel(true);
        }
    }


    public class PresenceIdentifier : IPresnceIdentifier
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ConcurrentDictionary<string, PersonState> peopleState = new ConcurrentDictionary<string, PersonState>();
        private ConcurrentDictionary<IDeviceDetails, string> devicesState = new ConcurrentDictionary<IDeviceDetails, string>();
        private PersonStateConfiguration DEFAULT_CONFIGURATION { get { return new PersonStateConfiguration(); } }

        public event EventHandler<string> PersonArrived;
        public event EventHandler<string> PersonLeft;
        private IDevicePresenceFactory devicePresenceFactory;

        public PresenceIdentifier(IDevicePresenceFactory devicePresenceFactory)
        {
            this.devicePresenceFactory = devicePresenceFactory;
        }

        public void OnDeviceIdentified(object sender, DeviceIdentifiedEventArgs args)
        {
            string personDevice;
            if (devicesState.TryGetValue(args.deviceDeatils, out personDevice))
            {
                DeviceConnected(personDevice);
            }
        }

        public void DeviceConnected(string personName)
        {
            DateTime connectionTime = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(personName)) return;
            if (!peopleState.ContainsKey(personName)) return;

            PersonState person = peopleState[personName];

            bool isPresent = person.IsPresent();
            person.lastSeen = connectionTime;
            person.configuration.cancelPresenceTimeout();

            if (!isPresent)
            {
                Task.Run(() => HandlePersonArrived(person.name));
            }

            var presenceCancellationTask = Task.Run(async delegate
            {
                try
                {
                    await Task.Delay(
                        person.configuration.MaximumAllowedDisconnection,
                        person.configuration.resetTimeoutCancellation().Token);
                    log.WarnFormat("Prensce timeout wasnt cancelled for {0}-{1}, starting handlepersonleft", person.name, person.lastSeen.ToShortDateString()+person.lastSeen.ToShortTimeString());
                    HandlePersonLeft(person.name);
                }
                catch (Exception e)
                {

                }
            });
        }

        private void HandlePersonLeft(string personName)
        {
            var evt = PersonLeft;
            if (evt != null) evt(this, personName);
        }

        private void HandlePersonArrived(string personName)
        {
            var evt = PersonArrived;
            if (evt != null) evt(this, personName);
        }


        public IDictionary<string, PersonState> getState()
        {
            return peopleState.Select(kvp => kvp).ToDictionary((kvp) => kvp.Key, (kvp2) => kvp2.Value);
        }

        public void registerPerson(PersonRegistration registration)
        {
            var personName = registration.personName;
            var configuration = registration.configuration;
            if (string.IsNullOrWhiteSpace(personName)) throw new ArgumentNullException("personName");
            if (registration.devicesDetails == null || registration.devicesDetails.Count() == 0) throw new ArgumentException("No devices found for person");
            bool devicesRegistered = false;
            foreach (var deviceDetails in registration.devicesDetails)
            {
                var identifier = devicePresenceFactory.Create(deviceDetails);
                if (identifier != null)
                {
                    if (devicesState.ContainsKey(deviceDetails)) throw new ArgumentException("Device already exists", deviceDetails.DeviceName);
                    devicesState.TryAdd(deviceDetails, personName);
                    identifier.RegisterDevice(deviceDetails);
                    identifier.OnDeviceIdentified += OnDeviceIdentified;
                    devicesRegistered = true;
                }
            }
            if (!devicesRegistered) throw new ArgumentException("No device identifiers found");

            var state = new PersonState(personName, configuration ?? DEFAULT_CONFIGURATION);

            peopleState.AddOrUpdate(personName, state, (k, v) => state);

        }
    }
}
