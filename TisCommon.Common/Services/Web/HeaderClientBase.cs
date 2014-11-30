// © 2008 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.ServiceModel;
using System.ServiceModel.Channels;


namespace TiS.Core.TisCommon.Services.Web
{
    public abstract class HeaderClientBase<T, H> : InterceptorClientBase<T> where T : class
    {
        protected H Header
        {
            get;
            set;
        }
        public HeaderClientBase()
            : this(default(H))
        { }

        public HeaderClientBase(ClientChannelFactoryConfiguration configuration)
            : this(default(H), configuration)
        { }

        public HeaderClientBase(ClientChannelFactoryConfiguration configuration, EndpointAddress remoteAddress)
            : this(default(H), configuration, remoteAddress)
        { }

        public HeaderClientBase(Binding binding, EndpointAddress remoteAddress)
            : this(default(H), binding, remoteAddress)
        { }

        public HeaderClientBase(H header)
        {
            Header = header;
        }

        public HeaderClientBase(H header, ClientChannelFactoryConfiguration configuration)
            : base(configuration)
        {
            Header = header;
        }

        public HeaderClientBase(H header, ClientChannelFactoryConfiguration configuration, EndpointAddress remoteAddress)
            : base(configuration, remoteAddress)
        {
            Header = header;
        }

        public HeaderClientBase(H header, Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
            Header = header;
        }

        protected override void PreInvoke(ref Message request)
        {
            GenericContext<H> context = new GenericContext<H>(Header);
            MessageHeader<GenericContext<H>> genericHeader =
                                          new MessageHeader<GenericContext<H>>(context);
            request.Headers.Add(genericHeader.GetUntypedHeader(
                       GenericContext<H>.TypeName, GenericContext<H>.TypeNamespace));
        }
    }
}