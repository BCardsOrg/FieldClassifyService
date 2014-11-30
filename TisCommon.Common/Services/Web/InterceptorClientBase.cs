// © 2008 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Linq;

namespace TiS.Core.TisCommon.Services.Web
{
    public abstract class InterceptorClientBase<T> : ChannelFactory<T> where T : class
    {
        private static ClientChannelFactoryConfiguration m_clientChannelFactoryConfiguration;

        public InterceptorClientBase()
        {
            InnerChannel = this.CreateChannel();
        }

        public InterceptorClientBase(ClientChannelFactoryConfiguration configuration)
            : base(StoreConfiguration(configuration))
        {
            InnerChannel = this.CreateChannel();
        }

        public InterceptorClientBase(ClientChannelFactoryConfiguration configuration, EndpointAddress remoteAddress)
            : base(StoreConfiguration(configuration), remoteAddress)
        {
            InnerChannel = this.CreateChannel();
        }

        public InterceptorClientBase(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
            InnerChannel = this.CreateChannel();
        }

        public T InnerChannel { get; set; }

        protected override void ApplyConfiguration(string configurationName)
        {
            if (m_clientChannelFactoryConfiguration.IsCustomConfig && m_clientChannelFactoryConfiguration.ServiceModelSection != null)
            {
                ChannelEndpointElement selectedEndpoint = null;

                foreach (ChannelEndpointElement endpoint in m_clientChannelFactoryConfiguration.ServiceModelSection.Client.Endpoints)
                {
                    if (StringUtil.CompareIgnoreCase(endpoint.Name, configurationName))
                    {
                        selectedEndpoint = endpoint;
                        break;
                    }
                }

                if (selectedEndpoint != null)
                {
                    if (this.Endpoint.Binding == null)
                    {
                        this.Endpoint.Binding = ServicesUtil.CreateBinding(
                            selectedEndpoint.Binding,
                            selectedEndpoint.BindingConfiguration,
                            m_clientChannelFactoryConfiguration.ServiceModelSection);
                    }

                    if (this.Endpoint.Address == null)
                    {
                        this.Endpoint.Address = new EndpointAddress(
                            selectedEndpoint.Address,
                            GetIdentity(selectedEndpoint.Identity),
                            selectedEndpoint.Headers.Headers);
                    }

                    if (this.Endpoint.Behaviors.Count == 0 && selectedEndpoint.BehaviorConfiguration != null)
                    {
                        ServicesUtil.AddBehaviors(
                            selectedEndpoint.BehaviorConfiguration,
                            this.Endpoint,
                            m_clientChannelFactoryConfiguration.ServiceModelSection);
                    }

                    this.Endpoint.Name = selectedEndpoint.Name;
                }
             }

            EnsureClientInterceptorBehavior(this.Endpoint);

            if (!m_clientChannelFactoryConfiguration.IsCustomConfig)
            {
                base.ApplyConfiguration(configurationName);
            }

            if (m_clientChannelFactoryConfiguration.Credentials != null)
            {
                EnsureClientCredentials(
                    this.Endpoint,
                    m_clientChannelFactoryConfiguration.Credentials.UserName.UserName,
                    m_clientChannelFactoryConfiguration.Credentials.UserName.Password);
            }
        }

        protected virtual void PreInvoke(ref Message request)
        {
        }

        protected virtual void PostInvoke(ref Message reply)
        {
        }

        private static string StoreConfiguration(ClientChannelFactoryConfiguration configuration)
        {
            m_clientChannelFactoryConfiguration = configuration;

            return m_clientChannelFactoryConfiguration.EndPointConfigurationName;
        }

        private void EnsureClientInterceptorBehavior(ServiceEndpoint endpoint)
        {
            ClientInterceptor item = endpoint.Behaviors.Find<ClientInterceptor>();

            if (item == null)
            {
                endpoint.Behaviors.Add(new ClientInterceptor(this));
            }
        }

        private void EnsureClientCredentials(
            ServiceEndpoint endpoint,
            string userName,
            string password)
        {
            ClientCredentials item = endpoint.Behaviors.Find<ClientCredentials>();

            if (item == null)
            {
                item = new ClientCredentials();

                endpoint.Behaviors.Add(item);
            }

            item.UserName.UserName = userName;
            item.UserName.Password = password;
        }

        private EndpointIdentity GetIdentity(IdentityElement element)
        {
            EndpointIdentity identity = null;

            PropertyInformationCollection properties = element.ElementInformation.Properties;

            if (properties["userPrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
            }

            if (properties["servicePrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
            }

            if (properties["dns"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
            }

            if (properties["rsa"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
            }

            if (properties["certificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                X509Certificate2Collection supportingCertificates = new X509Certificate2Collection();

                supportingCertificates.Import(Convert.FromBase64String(element.Certificate.EncodedValue));

                if (supportingCertificates.Count == 0)
                {
                    throw new InvalidOperationException("UnableToLoadCertificateIdentity");
                }

                X509Certificate2 primaryCertificate = supportingCertificates[0];

                supportingCertificates.RemoveAt(0);

                return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate, supportingCertificates);
            }

            return identity;
        }

        #region ClientInterceptor

        class ClientInterceptor : IEndpointBehavior, IClientMessageInspector
        {
            internal ClientInterceptor(InterceptorClientBase<T> proxy)
            {
                Proxy = proxy;
            }

            InterceptorClientBase<T> Proxy { get; set; }

            object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                Proxy.PreInvoke(ref request);
                return null;
            }

            void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
            {
                Proxy.PostInvoke(ref reply);
            }

            void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(this);
            }

            void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
            {
            }
        }

        #endregion
    }
}