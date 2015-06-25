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
using Helpers;

namespace HomeControl
{
    public class PersonStateConfiguration
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //public TimeSpan MaximumAllowedDisconnection = TimeSpan.FromSeconds(19);
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
            try
            {
                if (presenceTimeoutCancellation != null) presenceTimeoutCancellation.Cancel(true);
                else log.Warn("CancelPresenceTimeout on null source");
            }
            catch (Exception e)
            {
                log.Error("Error cancelling presence timeout", e);
            }
        }
    }


    public class PresenceIdentifier : IPresnceIdentifier
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ConcurrentDictionary<string, PersonState> peopleState = new ConcurrentDictionary<string, PersonState>();
        private ConcurrentDictionary<IDeviceDetails, string> devicesState = new ConcurrentDictionary<IDeviceDetails, string>();
        private PersonStateConfiguration DEFAULT_CONFIGURATION { get { return new PersonStateConfiguration(); } }
        //private readonly int PRESENCE_MONITOR_INTERVAL_MS = 10 * 1000;
        private readonly int PRESENCE_MONITOR_INTERVAL_MS = 60 * 1000;

        public event EventHandler<string> PersonArrived;
        public event EventHandler<string> PersonLeft;
        private IDevicePresenceFactory devicePresenceFactory;

        public PresenceIdentifier(IDevicePresenceFactory devicePresenceFactory)
        {
            this.devicePresenceFactory = devicePresenceFactory;
            Helper.StartRepetativeTask(monitorPresence, TimeSpan.FromMilliseconds(PRESENCE_MONITOR_INTERVAL_MS));
        }

        private void monitorPresence()
        {
            var now = DateTime.UtcNow;
            var peopleLeft = new List<string>();
            foreach (var person in peopleState)
            {
                if (!person.Value.IsPresent() && person.Value.lastSeen > person.Value.lastLeft)
                {
                    person.Value.lastLeft = now;
                    peopleLeft.Add(person.Value.name);
                }
            }

            if (peopleLeft.Count > 0) Helper.StartTask(() => announcePeopleLeft(peopleLeft), TimeSpan.FromMilliseconds(PRESENCE_MONITOR_INTERVAL_MS));
        }

        private void announcePeopleLeft(List<string> peopleLeft)
        {
            foreach (var personName in peopleLeft)
            {
                HandlePersonLeft(personName);
            }
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
            if (string.IsNullOrWhiteSpace(personName)) { log.Warn("DeviceConnected for empty personname"); return; }
            if (!peopleState.ContainsKey(personName)) { log.WarnFormat("DeviceConnected for unregistered personname {0}", personName); return; }

            PersonState person = peopleState[personName];

            bool isPresent = person.IsPresent();
            person.lastSeen = connectionTime;

            if (!isPresent)
            {
                Task.Run(() => HandlePersonArrived(person.name));
            }
        }

        private static void logAndThrowNonCancellationException(string personName, Exception e)
        {
            if (!(e is TaskCanceledException))
            {
                log.ErrorFormat(string.Format("Error is cancellation task for {0}", personName ?? "null"), e);
                throw e;
            }
        }

        private void HandlePersonLeft(string personName)
        {
            Helper.RaiseSafely(this, PersonLeft, personName);
        }

        private void HandlePersonArrived(string personName)
        {
            Helper.RaiseSafely(this, PersonArrived, personName);
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
                var identifier = devicePresenceFactory.GetOrCreate(deviceDetails);
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
