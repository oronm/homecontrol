using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Detection
{
    public interface ISensor
    {
        void Start();
        bool Stop();

        void StartProbeNow();

        IDetectable Register(IDetectable deviceDetails);
        bool Unregister(IDetectable deviceDetails, out IDetectable removedDeviceDetails);

        event EventHandler<DetectionEventArgs> Detection;
    }
}
