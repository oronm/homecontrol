using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Helpers;
using HomeControl;
using HomeControl.Local.Contracts;
using log4net;

namespace HomeControl.Local
{
    public class LocalHomeControlService : ILocalHomeControlService
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IHomeController homeController;
        private readonly IStateReportersFactory reportersFactory;
        private readonly IEnumerable<IStateReporter> reporters;
        private IEnumerable<PersonState> latestState;
        private System.Threading.CancellationTokenSource stateReportingTask;

        public event EventHandler<Contracts.PersonPresenceChangedEventArgs> OnPersonArrived;
        public event EventHandler<Contracts.PersonPresenceChangedEventArgs> OnPersonLeft;

        public LocalHomeControlService(IHomeController homeController, IStateReportersFactory reportersFactory)
        {
            this.homeController = homeController;
            this.reportersFactory = reportersFactory;
            this.reporters = reportersFactory.GetRegisteredStateReporters();
        }
        public void Start()
        {
            log.Info("Starting");
            homeController.OnPersonArrived += homeController_OnPersonArrived;
            homeController.OnPersonLeft += homeController_OnPersonLeft;
            stopStateReporting();
            startStateReporting();
        }
        public bool Stop()
        {
            log.Info("Stopping");
            homeController.OnPersonArrived -= homeController_OnPersonArrived;
            homeController.OnPersonLeft -= homeController_OnPersonLeft;
            stopStateReporting();
            return true;
        }
        public IEnumerable<PersonState> GetState()
        {
            return this.State;
        }

        protected IEnumerable<PersonState> State
        {
            get
            {
                var contollerState = this.homeController.GetState();
                this.latestState = contollerState == null || contollerState.Count() == 0 ?
                    new PersonState[] { } :
                    contollerState.Select(personState => createPersonState(personState));
                return this.latestState;
            }
        }

        private void homeController_OnPersonLeft(object sender, HomeControl.PersonPresenceChangedEventArgs e)
        {
            Notify(this.OnPersonLeft, e);
        }
        private void homeController_OnPersonArrived(object sender, HomeControl.PersonPresenceChangedEventArgs e)
        {
            Notify(this.OnPersonArrived, e);
        }
        private void Notify(EventHandler<Contracts.PersonPresenceChangedEventArgs> handler, HomeControl.PersonPresenceChangedEventArgs e)
        {
            var tmp = handler;
            var eventArgs = new Contracts.PersonPresenceChangedEventArgs(e);
            if (tmp != null) tmp(this, eventArgs);

            var personState = this.State.FirstOrDefault(ps => ps.name == e.Name);
            if (personState != null)
            {
                Parallel.ForEach(reporters, reporter =>
                {
                    try
                    {
                        log.DebugFormat("Reporting person state for {0} with {1}", personState.name, reporter);
                        reporter.ReportPersonState(personState);
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Error reporting presence for {0} with reporter {1}", personState.name, reporter.ToString()), ex);
                    }
                });
            }
        }

        private PersonState createPersonState(PresenceManager.PersonState personState)
        {
            if (personState == null) return null;
            return new PersonState()
            {
                name = personState.name,
                lastSeen = personState.lastSeen,
                lastLeft = personState.lastLeft,
                IsPresent = personState.IsPresent()
            };
        }

        private void startStateReporting()
        {
            this.stateReportingTask = Helper.StartRepetativeTask(reportLatestState, TimeSpan.FromSeconds(10));
        }
        private void stopStateReporting()
        {
            if (this.stateReportingTask != null && !this.stateReportingTask.IsCancellationRequested)
            {
                this.stateReportingTask.Cancel();
            }
        }
        private void reportLatestState()
        {
            var state = this.State;
            //Parallel.ForEach(reporters, reporter => { 
            foreach (var reporter in reporters)
	{
                try
                {
                    log.DebugFormat("Reporting location state with {0}", reporter);
                    reporter.ReportLocationState(state).ContinueWith(cont => { 
                        if (cont.IsFaulted)
                        {
                            log.Error(string.Format("Error reporting state with reporter {0}", reporter.ToString()), cont.Exception);
                        }
                    });
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Error reporting state with reporter {0}", reporter.ToString()), e);
                }
            };
        }
    }
}
