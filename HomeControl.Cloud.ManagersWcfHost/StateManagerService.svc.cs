using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using HomeControl.Cloud.Contracts;
using HomeControl.Cloud.Managers;
using log4net;

namespace HomeControl.Cloud.ManagersWcfHost
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class StateManagerService : StateManager, IStateReport, IStateFeed
    {
        static StateManagerService()
	    {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(@".\Bin\log4net.config"));
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Info("Starting");
	    }
    }
}
