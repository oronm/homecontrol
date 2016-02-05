using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HomeControl.Detection;

namespace HomeControl.Detection
{
    public class DetectionEventArgs
    {
        public IDetectable Detectable;
        public DateTime DetectionTimeUTC;
    }
}
