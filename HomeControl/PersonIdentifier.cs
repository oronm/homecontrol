using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl
{
    public class PersonStateConfiguration
    {
        //public TimeSpan MaximumAllowedDisconnection = TimeSpan.FromSeconds(15);
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
            if (presenceTimeoutCancellation != null) presenceTimeoutCancellation.Cancel(false);
        }
    }


    public class PersonIdentifier : IPresnceIdentifier
    {
        private ConcurrentDictionary<string, PersonState> peopleState = new ConcurrentDictionary<string, PersonState>();
        private PersonStateConfiguration DEFAULT_CONFIGURATION { get { return new PersonStateConfiguration(); } }

        public event EventHandler<string> PersonArrived;
        public event EventHandler<string> PersonLeft;

        public void PersonConnected(string personName)
        {
            DateTime connectionTime = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(personName)) return;

            if (!peopleState.ContainsKey(personName)) registerPerson(personName);
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

        public void registerPerson(string personName, PersonStateConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(personName)) throw new ArgumentNullException("personName");
            var state = new PersonState(personName, configuration);

            peopleState.AddOrUpdate(personName, state, (k,v) => state);
        }

        public void registerPerson(string personName)
        {
            registerPerson(personName, DEFAULT_CONFIGURATION);
        }
    }
}
