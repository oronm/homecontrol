using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl
{
    public interface IResidentsRepository
    {
        IEnumerable<IdentifyingDevice> GetIdentifyingDevices();
    }
}
