using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace TiS.Core.TisCommon.Services.Web
{
    #region TisWebServiceClient

    public class TisWebServiceClient : IDisposable
    {
        private Type m_contractType;
        private Type m_genericHeaderClientBase;
        private PropertyInfo m_innerChannelProperty;
        //     private PropertyInfo m_headerProperty;
        private ITisWebServiceClient<TisWebServiceContextData> m_webServiceClient;
        //     private CommunicationObject m_CommunicationObjectClient;
        private IClientChannel m_innerChannel;
        private static GacAssemblyResolver m_gacAssemblyResolver = new GacAssemblyResolver();

        public TisWebServiceClient(
            string contractType,
            ClientChannelFactoryConfiguration configuration,
            TisWebServiceContextData contextData,
            string webServiceUri)
            : this(GetVersionedType(contractType), configuration, contextData, webServiceUri)
        {
        }

        private static Type GetVersionedType(string contractType)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(m_gacAssemblyResolver.AssemblyResolveHandler);
                return Type.GetType(contractType);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(m_gacAssemblyResolver.AssemblyResolveHandler);
            }
        }

        public TisWebServiceClient(
            Type contractType,
            ClientChannelFactoryConfiguration configuration,
            TisWebServiceContextData contextData,
            string webServiceUri)
            : this(contractType)
        {
            CreateWebServiceClient(contextData, configuration, webServiceUri);
        }

        ~TisWebServiceClient()
        {
            Dispose(false);
        }

        private TisWebServiceClient(Type contractType)
        {
            object[] attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false);

            if (attributes.Length != 1)
            {
                throw new TisException("Type of " + contractType + " is not a service contract");
            }

            m_contractType = contractType;
            m_genericHeaderClientBase = typeof(TisWebServiceClient<,>).MakeGenericType(m_contractType, typeof(TisWebServiceContextData));
            m_innerChannelProperty = m_genericHeaderClientBase.GetProperty("InnerChannel");
        }

        public object ClientChannel
        {
            get
            {
                return m_innerChannel;
            }
        }

        public object Header
        {
            get
            {
                return m_webServiceClient.Header;
            }
            set
            {
                m_webServiceClient.Header = value as TisWebServiceContextData;
            }
        }

        private object CreateWebServiceClient(
            params object[] headerClientBaseCreationParams)
        {
            try
            {
                m_webServiceClient = Activator.CreateInstance(m_genericHeaderClientBase, headerClientBaseCreationParams) as ITisWebServiceClient<TisWebServiceContextData>;
                m_innerChannel = (IClientChannel)m_innerChannelProperty.GetValue(m_webServiceClient, null);

                return m_innerChannel;
            }
            catch (Exception exc)
            {
                Log.WriteException(exc);
            }

            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_webServiceClient != null)
                {
                    m_webServiceClient.Close();
                }

                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }

    #endregion

    #region TisGenericServiceClient<T, H>

    /// <summary>
    /// This interface comes to avoid reflection when setting the header.
    /// </summary>
    public interface ITisWebServiceClient<H>
    {
        H Header { get; set; }
        void Close();
    }

    public class TisWebServiceClient<T, H> : HeaderClientBase<T, H>, ITisWebServiceClient<H> where T : class
    {
        public TisWebServiceClient(H header, ClientChannelFactoryConfiguration configuration, string webServiceUri)
            : base(header, configuration)
        {
            if (this.Endpoint.Address == null)
            {
                this.Endpoint.Address = new EndpointAddress(webServiceUri);
            }
        }

        H ITisWebServiceClient<H>.Header
        {
            get
            {
                return base.Header;
            }
            set
            {
                base.Header = value;
            }
        }

        void ITisWebServiceClient<H>.Close()
        {
            if (this.State != CommunicationState.Closing && this.State != CommunicationState.Closed)
            {
                Close();
            }
        }
    }

    #endregion
}
