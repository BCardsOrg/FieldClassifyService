using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace TiS.Core.TisCommon.Services.Web
{
    public class ServicesUtil
    {
        public static void AddBehaviors(
            string behaviorConfiguration,
            ServiceEndpoint serviceEndpoint,
            ServiceModelSectionGroup serviceModelSection)
        {
            EndpointBehaviorElement behaviorElement = serviceModelSection.Behaviors.EndpointBehaviors[behaviorConfiguration];

            for (int i = 0; i < behaviorElement.Count; i++)
            {
                BehaviorExtensionElement behaviorExtension = behaviorElement[i];

                IEndpointBehavior endPointBehavior = CreateBehavior(behaviorExtension);

                if (endPointBehavior != null)
                {
                    serviceEndpoint.Behaviors.Add(endPointBehavior);
                }
            }
        }

        public static  IEndpointBehavior CreateBehavior(BehaviorExtensionElement behaviorExtension)
        {
            IEndpointBehavior endPointBehavior = (IEndpointBehavior)behaviorExtension.GetType().InvokeMember(
                "CreateBehavior",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                behaviorExtension,
                null);

            return endPointBehavior;
        }

        public static Binding CreateBinding(
            string bindingSectionName,
            string bindingName,
            ServiceModelSectionGroup serviceModelSection)
        {
            Binding binding = null;

            BindingCollectionElement bindingElementCollection = serviceModelSection.Bindings[bindingSectionName];

            IBindingConfigurationElement bindingElement =
                bindingElementCollection.ConfiguredBindings.First(be => StringUtil.CompareIgnoreCase(be.Name, bindingName));

            if (bindingElement != null)
            {
                binding = GetBinding(bindingElement);

                bindingElement.ApplyConfiguration(binding);
            }

            return binding;
        }

        public static Binding GetBinding(IBindingConfigurationElement configurationElement)
        {
            if (configurationElement is CustomBindingElement)
                return new CustomBinding();
            else if (configurationElement is BasicHttpBindingElement)
                return new BasicHttpBinding();
            else if (configurationElement is NetMsmqBindingElement)
                return new NetMsmqBinding();
            else if (configurationElement is NetNamedPipeBindingElement)
                return new NetNamedPipeBinding();
            else if (configurationElement is NetPeerTcpBindingElement)
                return new NetPeerTcpBinding();
            else if (configurationElement is NetTcpBindingElement)
                return new NetTcpBinding();
            else if (configurationElement is WSDualHttpBindingElement)
                return new WSDualHttpBinding();
            else if (configurationElement is WSHttpBindingElement)
                return new WSHttpBinding();
            else if (configurationElement is WS2007FederationHttpBindingElement)
                return new WS2007FederationHttpBinding();
            else if (configurationElement is WSFederationHttpBindingElement)
                return new WSFederationHttpBinding();

            return null;
        }

        public static Binding GetBinding(
            Uri address, 
            string configurationName = null)
        {
            switch (address.Scheme)
            {
                case "net.tcp":
                    {
                        return StringUtil.IsStringInitialized(configurationName) ? new NetTcpBinding(configurationName) : new NetTcpBinding();
                    }
                case "net.pipe":
                    {
                        return StringUtil.IsStringInitialized(configurationName) ? new NetNamedPipeBinding(configurationName) : new NetNamedPipeBinding();
                    }
                case "net.msmq":
                    {
                        return StringUtil.IsStringInitialized(configurationName) ? new NetMsmqBinding(configurationName) : new NetMsmqBinding();
                    }
                default:
                    {
                        throw new InvalidOperationException("Can only create a channel over TCP/IPC/MSMQ bindings");
                    }
            }
        }
    }
}
