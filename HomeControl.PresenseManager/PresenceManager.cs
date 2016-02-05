using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Detection;
using Castle.Facilities.TypedFactory;
using log4net;
using Helpers;

namespace HomeControl.PresenceManager
{
    public class PresenceManager : IPresenceManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ConcurrentDictionary<string, PersonState> peopleState = new ConcurrentDictionary<string, PersonState>();
        private ConcurrentDictionary<IDetectable, string> detectablesState = new ConcurrentDictionary<IDetectable, string>();
        private PersonPresencePolicy DEFAULT_CONFIGURATION { get { return new PersonPresencePolicy(); } }
        //private readonly int PRESENCE_MONITOR_INTERVAL_MS = 10 * 1000;
        private readonly int PRESENCE_MONITOR_INTERVAL_MS = 60 * 1000;

        public event EventHandler<string> PersonArrived;
        public event EventHandler<string> PersonLeft;
        private ISensorFactory sensorsFactory;

        public PresenceManager(ISensorFactory sensonsFactory)
        {
            this.sensorsFactory = sensonsFactory;
            Helper.StartRepetativeTask(monitorPresence, TimeSpan.FromMilliseconds(PRESENCE_MONITOR_INTERVAL_MS));
        }

        public IDictionary<string, PersonState> GetState()
        {
            return peopleState.Select(kvp => kvp).ToDictionary((kvp) => kvp.Key, (kvp2) => kvp2.Value);
        }

        public void RegisterPerson(PersonRegistration registration)
        {
            var personName = registration.PersonName;
            var configuration = registration.PresencePolicy;
            if (string.IsNullOrWhiteSpace(personName)) throw new ArgumentNullException("personName");
            if (registration.Detectables == null || registration.Detectables.Count() == 0) throw new ArgumentException("No detectables found for person");

            bool detectableRegistered = false;
            foreach (var detectable in registration.Detectables)
            {
                if (detectablesState.ContainsKey(detectable)) throw new ArgumentException("detectable already exists", detectable.Description);

                var sensor = sensorsFactory.GetOrCreate(detectable);
                if (sensor != null)
                {
                    detectablesState.TryAdd(detectable, personName);
                    sensor.Register(detectable);
                    sensor.Detection += OnDetection;
                    sensor.Start();
                    detectableRegistered = true;
                }
            }
            if (!detectableRegistered) throw new ArgumentException("No supported sensors found");

            var state = new PersonState(personName, configuration ?? DEFAULT_CONFIGURATION);

            peopleState.AddOrUpdate(personName, state, (k, v) => state);

        }

        protected void OnDetection(object sender, DetectionEventArgs args)
        {
            string detectable;
            if (detectablesState.TryGetValue(args.Detectable, out detectable))
            {
                HandlePersonDetected(detectable);
            }
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

        private void HandlePersonDetected(string personName)
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
    }
}
