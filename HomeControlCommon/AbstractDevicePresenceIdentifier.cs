using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using log4net;

namespace HomeControl.Common
{
    public abstract class AbstractDevicePresenceIdentifier : IDevicePresenceIdentifier
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected ConcurrentDictionary<string, IDeviceDetails> registeredDevices = new ConcurrentDictionary<string, IDeviceDetails>();
        public IDeviceDetails RegisterDevice(IDeviceDetails deviceDetails)
        {
            throwIfDeviceDeatilsInvalid(deviceDetails);
            return registeredDevices.AddOrUpdate(deviceDetails.DeviceId, deviceDetails, (k, v) => deviceDetails);
        }
        public bool UnregisterDevice(IDeviceDetails deviceDetails, out IDeviceDetails removedDeviceDetails)
        {
            throwIfDeviceDeatilsInvalid(deviceDetails);
            return registeredDevices.TryRemove(deviceDetails.DeviceId, out removedDeviceDetails);
        }

        private static void throwIfDeviceDeatilsInvalid(IDeviceDetails deviceDetails)
        {
            if (deviceDetails == null) throw new ArgumentNullException("deviceDetails");
            if (String.IsNullOrWhiteSpace(deviceDetails.DeviceId)) throw new ArgumentNullException("DeviceId");
        }

        private event EventHandler<DeviceIdentifiedEventArgs> onDeviceIdentified;
        public event EventHandler<DeviceIdentifiedEventArgs> OnDeviceIdentified
        {
            add
            {
                if (value == null) throw new ArgumentNullException();
                if (this.onDeviceIdentified == null || !this.onDeviceIdentified.GetInvocationList().Contains(value))
                {
                    this.onDeviceIdentified += value;
                }
            }
            remove
            {
                if (value == null) throw new ArgumentNullException();
                this.onDeviceIdentified -= value;
            }
        }

        protected void InitiateOnDeviceIdentied(IEnumerable<IDeviceDetails> devices)
        {
            new Task(() =>
            {
                var evt = onDeviceIdentified;
                if (evt != null && devices != null && devices.Count() > 0)
                {
                    var now = DateTime.UtcNow;
                    foreach (var identifiedDevice in devices)
                    {
                        try
                        {
                            evt(this, new DeviceIdentifiedEventArgs() { deviceDeatils = identifiedDevice, presenceUtcTime = now });
                        }
                        catch (Exception e)
                        {
                            log.Error(string.Format("Error rasising OnDeviceIdentified for device {0}:{1}", identifiedDevice.DeviceId, identifiedDevice.DeviceName), e);
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning).Start();

        }
        protected abstract IEnumerable<IDeviceDetails> IdentifyDevicesPresence();

    }

    public abstract class AbstractTimedDevicePresenceIDentifier : AbstractDevicePresenceIdentifier
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AbstractTimedDevicePresenceIDentifier()
        {
            StartTimer();
        }

        private CancellationTokenSource cancellationTokenSource;
        private void StartTimer()
        {
            log.Info("Started device presence identification timer");
            if (cancellationTokenSource != null) { cancellationTokenSource.Cancel(); cancellationTokenSource = null; }
            cancellationTokenSource = Helper.StartRepetativeTask(() =>
                {
                    log.Info("Identiying deivces presence");
                    InitiateOnDeviceIdentied(IdentifyDevicesPresence());

                }, TimeSpan.FromSeconds(60));
        }
    }
}
