// © 2008 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Diagnostics;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Reflection;

namespace TiS.Core.TisCommon.Services.Web
{
    public class ServiceHost<T> : ServiceHost
    {
        class ErrorHandlerBehavior : IServiceBehavior, IErrorHandler
        {
            IErrorHandler m_ErrorHandler;
            public ErrorHandlerBehavior(IErrorHandler errorHandler)
            {
                m_ErrorHandler = errorHandler;
            }
            void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase host)
            { }
            void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase host, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
            { }
            void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase host)
            {
                foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
                {
                    dispatcher.ErrorHandlers.Add(this);
                }
            }
            bool IErrorHandler.HandleError(Exception error)
            {
                return m_ErrorHandler.HandleError(error);
            }
            void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref System.ServiceModel.Channels.Message fault)
            {
                m_ErrorHandler.ProvideFault(error, version, ref fault);
            }
        }

        private static ClientChannelFactoryConfiguration m_clientChannelFactoryConfiguration;

        List<IServiceBehavior> m_ErrorHandlers = new List<IServiceBehavior>();

        /// <summary>
        /// Can only call before openning the host
        /// </summary>
        public void AddErrorHandler(IErrorHandler errorHandler)
        {
            if (State == CommunicationState.Opened)
            {
                throw new InvalidOperationException("Host is already opened");
            }
            Debug.Assert(errorHandler != null);
            IServiceBehavior errorHandlerBehavior = new ErrorHandlerBehavior(errorHandler);

            m_ErrorHandlers.Add(errorHandlerBehavior);
        }
        /// <summary>
        /// Can only call before openning the host
        /// </summary>
        public void AddErrorHandler()
        {
            AddErrorHandler(new ErrorHandlerBehaviorAttribute());
        }

        /// <summary>
        /// Can only call before openning the host
        /// </summary>
        public bool EnableMetadataExchange
        {
            set
            {
                if (State == CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is already opened");
                }

                ServiceMetadataBehavior metadataBehavior;
                metadataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (metadataBehavior == null)
                {
                    metadataBehavior = new ServiceMetadataBehavior();
                    metadataBehavior.HttpGetEnabled = value;
                    Description.Behaviors.Add(metadataBehavior);
                }
                if (value == true)
                {
                    if (HasMexEndpoint == false)
                    {
                        AddAllMexEndPoints();
                    }
                }
            }
            get
            {
                ServiceMetadataBehavior metadataBehavior;
                metadataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (metadataBehavior == null)
                {
                    return false;
                }
                return metadataBehavior.HttpGetEnabled;
            }
        }
        public void AddAllMexEndPoints()
        {
            Debug.Assert(HasMexEndpoint == false);

            foreach (Uri baseAddress in BaseAddresses)
            {
                BindingElement bindingElement = null;
                switch (baseAddress.Scheme)
                {
                    case "net.tcp":
                        {
                            bindingElement = new TcpTransportBindingElement();
                            break;
                        }
                    case "net.pipe":
                        {
                            bindingElement = new NamedPipeTransportBindingElement();
                            break;
                        }
                    case "http":
                        {
                            bindingElement = new HttpTransportBindingElement();
                            break;
                        }
                    case "https":
                        {
                            bindingElement = new HttpsTransportBindingElement();
                            break;
                        }
                }
                if (bindingElement != null)
                {
                    Binding binding = new CustomBinding(bindingElement);
                    AddServiceEndpoint(typeof(IMetadataExchange), binding, "MEX");
                }
            }
        }
        public bool HasMexEndpoint
        {
            get
            {
                return Description.Endpoints.Any(endpoint => endpoint.Contract.ContractType == typeof(IMetadataExchange));
            }
        }

        protected override void OnOpening()
        {
            foreach (IServiceBehavior behavior in m_ErrorHandlers)
            {
                Description.Behaviors.Add(behavior);
            }
            base.OnOpening();
        }

        /// <summary>
        /// Can only call after openning the host
        /// </summary>
        public ServiceThrottle Throttle
        {
            get
            {
                if (State != CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is not opened");
                }

                ChannelDispatcher dispatcher = OperationContext.Current.Host.ChannelDispatchers[0] as ChannelDispatcher;
                return dispatcher.ServiceThrottle;
            }
        }
        /// <summary>
        /// Can only call before openning the host
        /// </summary>
        public bool IncludeExceptionDetailInFaults
        {
            set
            {
                if (State == CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is already opened");
                }
                ServiceBehaviorAttribute debuggingBehavior = Description.Behaviors.Find<ServiceBehaviorAttribute>();
                debuggingBehavior.IncludeExceptionDetailInFaults = value;
            }
            get
            {
                ServiceBehaviorAttribute debuggingBehavior = Description.Behaviors.Find<ServiceBehaviorAttribute>();
                return debuggingBehavior.IncludeExceptionDetailInFaults;
            }
        }

        /// <summary>
        /// Can only call before openning the host
        /// </summary>
        public bool SecurityAuditEnabled
        {
            get
            {
                ServiceSecurityAuditBehavior securityAudit = Description.Behaviors.Find<ServiceSecurityAuditBehavior>();
                if (securityAudit != null)
                {
                    return securityAudit.MessageAuthenticationAuditLevel == AuditLevel.SuccessOrFailure &&
                           securityAudit.ServiceAuthorizationAuditLevel == AuditLevel.SuccessOrFailure;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (State == CommunicationState.Opened)
                {
                    throw new InvalidOperationException("Host is already opened");
                }
                ServiceSecurityAuditBehavior securityAudit = Description.Behaviors.Find<ServiceSecurityAuditBehavior>();
                if (securityAudit == null && value == true)
                {
                    securityAudit = new ServiceSecurityAuditBehavior();
                    securityAudit.MessageAuthenticationAuditLevel = AuditLevel.SuccessOrFailure;
                    securityAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
                    Description.Behaviors.Add(securityAudit);
                }
            }
        }

        public ServiceHost()
            : base(typeof(T))
        { 
        }

        public ServiceHost(params string[] baseAddresses)
            : base(typeof(T), Convert(baseAddresses))
        { 
        }

        public ServiceHost(params Uri[] baseAddresses)
            : base(typeof(T), baseAddresses)
        {
        }

        public ServiceHost(T singleton)
            : base(singleton)
        {
        }

        public ServiceHost(ClientChannelFactoryConfiguration clientChannelFactoryConfiguration)
            : this(StoreConfiguration(clientChannelFactoryConfiguration))
        {
            m_clientChannelFactoryConfiguration = clientChannelFactoryConfiguration;
        }

        public ServiceHost(T singleton, ClientChannelFactoryConfiguration clientChannelFactoryConfiguration)
            : this(singleton, StoreConfiguration(clientChannelFactoryConfiguration))
        {
            m_clientChannelFactoryConfiguration = clientChannelFactoryConfiguration;
        }


        public ServiceHost(string dummy) : this()
        {
        }

        public ServiceHost(T singleton, string dummy)
            : this(singleton)
        {
        }

        public virtual T Singleton
        {
            get
            {
                if (SingletonInstance == null)
                {
                    return default(T);
                }
                Debug.Assert(SingletonInstance is T);
                return (T)SingletonInstance;
            }
        }
        static Uri[] Convert(string[] baseAddresses)
        {
            Converter<string, Uri> convert = (address) =>
                                            {
                                                return new Uri(address);
                                            };
            return Array.ConvertAll(baseAddresses, convert);
        }

        protected override void ApplyConfiguration()
        {
            if (m_clientChannelFactoryConfiguration.ServiceModelSection.Services.Services.Count > 0)
            {
                ServiceEndpointElement selectedEndpoint = null;

                foreach (ServiceElement serviceElement in m_clientChannelFactoryConfiguration.ServiceModelSection.Services.Services)
                {
                    if (StringUtil.CompareIgnoreCase(serviceElement.Name, this.Description.Name))
                    {
                        selectedEndpoint = serviceElement.Endpoints[0];
                        break;
                    }
                }

                if (selectedEndpoint != null)
                {
                    if (this.Description.Endpoints == null || this.Description.Endpoints.Count == 0)
                    {
                        if (StringUtil.CompareIgnoreCase(selectedEndpoint.Address.Host, MachineInfo.MachineName) == false)
                        {
                            // Ensure no "localhost" as Uri host name.
                            selectedEndpoint.Address = new Uri(selectedEndpoint.Address.ToString().Replace(selectedEndpoint.Address.Host, MachineInfo.MachineName));
                        }

                        var serviceEndpoint = new ServiceEndpoint(
                            ContractDescription.GetContract(typeof(T)),
                            ServicesUtil.GetBinding(selectedEndpoint.Address, selectedEndpoint.BindingConfiguration),
                            new EndpointAddress(selectedEndpoint.Address));

                        if (StringUtil.IsStringInitialized(selectedEndpoint.Name))
                        {
                            serviceEndpoint.Name = selectedEndpoint.Name;
                        }

                        this.Description.Endpoints.Add(serviceEndpoint);
                    }
                }
            }
            else
            {
                base.ApplyConfiguration();
            }
        }

        private static string StoreConfiguration(ClientChannelFactoryConfiguration configuration)
        {
            m_clientChannelFactoryConfiguration = configuration;

            return String.Empty;
        }
    }
}





