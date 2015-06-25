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
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
            try
            {
                DateTime connectionTime = DateTime.UtcNow;
                if (string.IsNullOrWhiteSpace(personName)) { log.Warn("DeviceConnected for empty personname"); return; }
                if (!peopleState.ContainsKey(personName)) { log.WarnFormat("DeviceConnected for unregistered personname {0}", personName); return; }

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
                        var cancellation = person.configuration.resetTimeoutCancellation();
                        if (cancellation.IsCancellationRequested || cancellation.Token.IsCancellationRequested)
                            log.WarnFormat("Presence timeout is entering with a cancelled token for {0} source={1} token={2}", person.name, cancellation.IsCancellationRequested, cancellation.Token.IsCancellationRequested);
                        
                        await Task.Delay(
                            person.configuration.MaximumAllowedDisconnection,
                            cancellation.Token);

                        log.WarnFormat("Presensce timeout wasnt cancelled for {0}-{1}, source={2} token={3}", person.name, person.lastSeen.ToShortDateString() + " " + person.lastSeen.ToShortTimeString(), cancellation.IsCancellationRequested, cancellation.Token.IsCancellationRequested);

                        if (!person.IsPresent()) HandlePersonLeft(person.name);
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var ex in ae.InnerExceptions)
                        {
                            logAndThrowNonCancellationException(personName, ex);
                        }
                    }
                    catch (Exception e)
                    {
                        logAndThrowNonCancellationException(personName, e);
                    }
                });
            }
            catch (Exception e)
            {
                log.Error(string.Format("General exception on DeviceConnected for {0}", personName ?? "null"), e);
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
