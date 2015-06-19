using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Common;

namespace HomeControl
{
    public struct PersonRegistration
    {
        public string personName;
        public IEnumerable<IDeviceDetails> devicesDetails;
        public PersonStateConfiguration configuration;
    }
}
