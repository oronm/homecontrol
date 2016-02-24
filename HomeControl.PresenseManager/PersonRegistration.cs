using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Detection;

namespace HomeControl.PresenceManager
{
    public struct PersonRegistration
    {
        public string PersonName;
        public IEnumerable<IDetectable> Detectables;
        public PersonPresencePolicy PresencePolicy;
    }

    public class PersonPresencePolicy
    {
        //private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //public TimeSpan MaximumAbsencyAllowed = TimeSpan.FromSeconds(10);
        public TimeSpan MaximumAbsencyAllowed = TimeSpan.FromMinutes(2);

        //private CancellationTokenSource presenceTimeoutCancellation;

        //private CancellationTokenSource resetTimeoutCancellation()
        //{
        //    cancelPresenceTimeout();
        //    presenceTimeoutCancellation = new CancellationTokenSource();
        //    return presenceTimeoutCancellation;
        //}
        //private void cancelPresenceTimeout()
        //{
        //    try
        //    {
        //        if (presenceTimeoutCancellation != null) presenceTimeoutCancellation.Cancel(true);
        //        else log.Warn("CancelPresenceTimeout on null source");
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error cancelling presence timeout", e);
        //    }
        //}
    }

}

