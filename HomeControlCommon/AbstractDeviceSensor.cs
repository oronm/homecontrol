using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using log4net;

namespace HomeControl.Detection
{
    public abstract class AbstractSensor : ISensor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected ConcurrentDictionary<string, IDetectable> registeredDetectables = new ConcurrentDictionary<string, IDetectable>();

        public IDetectable Register(IDetectable detectable)
        {
            throwIfDetectionDeatilsInvalid(detectable);
            return registeredDetectables.AddOrUpdate(detectable.Identification, detectable, (k, v) => detectable);
        }
        public bool Unregister(IDetectable detectable, out IDetectable removedDetectable)
        {
            throwIfDetectionDeatilsInvalid(detectable);
            return registeredDetectables.TryRemove(detectable.Identification, out removedDetectable);
        }

        public event EventHandler<DetectionEventArgs> Detection
        {
            add
            {
                if (value == null) throw new ArgumentNullException();
                if (this.onDetection == null || !this.onDetection.GetInvocationList().Contains(value))
                {
                    this.onDetection += value;
                }
            }
            remove
            {
                if (value == null) throw new ArgumentNullException();
                this.onDetection -= value;
            }
        }

        public abstract void Start();
        public abstract bool Stop();
        protected abstract bool IsStarted { get; }
        public void StartProbeNow()
        {
            InitiateDetectionEvent(Detect());
        }
        
        private static void throwIfDetectionDeatilsInvalid(IDetectable detectable)
        {
            if (detectable == null) throw new ArgumentNullException("deviceDetails");
            if (String.IsNullOrWhiteSpace(detectable.Identification)) throw new ArgumentNullException("DeviceId");
        }

        private event EventHandler<DetectionEventArgs> onDetection;

        protected void InitiateDetectionEvent(IEnumerable<IDetectable> detectables)
        {
            new Task(() =>
            {
                var evt = onDetection;
                if (evt != null && detectables != null && detectables.Count() > 0)
                {
                    var now = DateTime.UtcNow;
                    foreach (var identifiedDevice in detectables)
                    {
                        try
                        {
                            evt(this, new DetectionEventArgs() { Detectable = identifiedDevice, DetectionTimeUTC = now });
                        }
                        catch (Exception e)
                        {
                            log.Error(string.Format("Error rasising OnDeviceIdentified for device {0}:{1}", identifiedDevice.Identification, identifiedDevice.Description), e);
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning).Start();

        }
        protected abstract IEnumerable<IDetectable> Detect();
    }

    public abstract class AbstractTimedSensor : AbstractSensor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource cancellationTokenSource;
        private void StartTimer()
        {
            log.Info("Started detection timer");
            cancellationTokenSource = Helper.StartRepetativeTask(() =>
                {
                    log.Debug("Probing");
                    StartProbeNow();

                }, TimeSpan.FromSeconds(30));
        }

        protected override bool IsStarted
        {
            get
            {
                return cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested;
            }
        }
        public override void Start()
        {
            if (!IsStarted)
            {
                StartTimer();
            }
        }
        public override bool Stop()
        {
            if (cancellationTokenSource != null)
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                }
                cancellationTokenSource = null;
            }

            return true;
        }
    }
}
