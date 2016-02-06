using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace HomeControl.Cloud.ManagersBasicHost
{
    public class WcfServiceWrapper<TServiceImplementation, TServiceContract>
        : ServiceBase
        where TServiceImplementation : TServiceContract
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _serviceUri;
        private ServiceHost _serviceHost;
        private TServiceContract _instance;

        public WcfServiceWrapper(TServiceContract instance)
        {
            this._instance = instance;
        }

        protected override void OnStart(string[] args)
        {
            Start(args[0], args[1]);
        }

        protected override void OnStop()
        {
            Stop();
        }

        public void Start(string serviceName, string serviceUri)
        {
            _serviceUri = serviceUri;
            ServiceName = serviceName;

            log.Info(ServiceName + " starting...");
            bool openSucceeded = false;
            try
            {
                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                }

                // TODO : Convert svc wrapper to work with custom host factory
                _serviceHost = new ServiceHost(_instance);
                //_serviceHost = new ServiceHost(typeof(TServiceImplementation));
            }
            catch (Exception e)
            {
                log.Info("Caught exception while creating " + ServiceName + ": " + e);
                return;
            }

            try
            {
                var webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);
                _serviceHost.AddServiceEndpoint(typeof(TServiceContract), webHttpBinding, _serviceUri);
                
                var webHttpBehavior = new WebHttpBehavior
                {
                    DefaultBodyStyle = WebMessageBodyStyle.Wrapped,
                    DefaultOutgoingResponseFormat = WebMessageFormat.Json,
                    DefaultOutgoingRequestFormat = WebMessageFormat.Json
                };
                _serviceHost.Description.Endpoints[0].Behaviors.Add(webHttpBehavior);

                _serviceHost.Open();
                openSucceeded = true;
            }
            catch (Exception ex)
            {
                log.Error("Caught exception while starting " + ServiceName, ex);
            }
            finally
            {
                if (!openSucceeded)
                {
                    _serviceHost.Abort();
                }
            }

            if (_serviceHost.State == CommunicationState.Opened)
            {
                log.Info(ServiceName + " started at " + _serviceUri);
            }
            else
            {
                log.Info(ServiceName + " failed to open");
                bool closeSucceeded = false;
                try
                {
                    _serviceHost.Close();
                    closeSucceeded = true;
                }
                catch (Exception ex)
                {
                    log.Error(ServiceName + " failed to close ",ex);
                }
                finally
                {
                    if (!closeSucceeded)
                    {
                        _serviceHost.Abort();
                    }
                }
            }
        }

        public new void Stop()
        {
            log.Info(ServiceName + " stopping...");
            try
            {
                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                    _serviceHost = null;
                }
            }
            catch (Exception ex)
            {
                log.Error("Caught exception while stopping " + ServiceName, ex);
            }
            finally
            {
                log.Info(ServiceName + " stopped...");
            }
        }
    }
}
