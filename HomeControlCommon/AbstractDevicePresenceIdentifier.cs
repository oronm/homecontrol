using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Common
{
    public abstract class AbstractDevicePresenceIdentifier : IDevicePresenceIdentifier
    {
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

        public event EventHandler<DeviceIdentifiedEventArgs> OnDeviceIdentified;

        protected void InitiateOnDeviceIdentied(IEnumerable<IDeviceDetails> devices)
        {
            new Task(() => {
                var evt = OnDeviceIdentified;
                if (evt != null && devices != null && devices.Count() > 0)
                {
                    var now = DateTime.UtcNow;
                    foreach (var identifiedDevice in devices)
                    {
                        try
                        {
                            OnDeviceIdentified(this, new DeviceIdentifiedEventArgs() { deviceDeatils = identifiedDevice, presenceUtcTime = now });
                        }
                        catch
                        {

                        }
                    }
                }
            }, TaskCreationOptions.LongRunning).Start();

        }
        protected abstract IEnumerable<IDeviceDetails> IdentifyDevicesPresence();

    }

    public abstract class AbstractTimedDevicePresenceIDentifier : AbstractDevicePresenceIdentifier
    {
        public AbstractTimedDevicePresenceIDentifier ()
        {
            StartTimer();
        }

        private Task identificationTask;
        private void StartTimer()
        {
            if (identificationTask != null) { identificationTask.Dispose(); identificationTask = null; }
            identificationTask = new Task(() =>
            {
                while (true)
                {
                    InitiateOnDeviceIdentied(IdentifyDevicesPresence());
                    Thread.Sleep(1000 * 15);
                }
            }, TaskCreationOptions.LongRunning);
            identificationTask.Start();

        }
    }
}
