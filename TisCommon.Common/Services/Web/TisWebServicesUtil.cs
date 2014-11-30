using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Reflection;
using System.Collections;
using System.Configuration;

namespace TiS.Core.TisCommon.Services.Web
{

    #region TisServiceExtensions

    public static class TisWebServiceExtensions
    {
        public const string COMMUNICATION_PROTOCOL_HTTP = "BasicHttp";
        public const string COMMUNICATION_PROTOCOL_TCP = "NetTcp";
        public const string COMMUNICATION_PROTOCOL_WS_HTTP = "WSHttp";
        public const string COMMUNICATION_PROTOCOL_NET_PIPE = "NetPipe";

        public const string ENDPOINT_SCHEME_WS_HTTP = "http";
        public const string ENDPOINT_SCHEME_HTTP = "http";
        public const string ENDPOINT_SCHEME_TCP = "net.tcp";
        public const string ENDPOINT_SCHEME_NET_PIPE = "net.pipe";

        public static string SchemeToString(this ServiceEndpoint endpoint)
        {
            if (StringUtil.CompareIgnoreCase(endpoint.ListenUri.Scheme, ENDPOINT_SCHEME_WS_HTTP))
            {
                return COMMUNICATION_PROTOCOL_WS_HTTP;
            }
            if (StringUtil.CompareIgnoreCase(endpoint.ListenUri.Scheme, ENDPOINT_SCHEME_HTTP))
            {
                return COMMUNICATION_PROTOCOL_HTTP;
            }
            if (StringUtil.CompareIgnoreCase(endpoint.ListenUri.Scheme, ENDPOINT_SCHEME_TCP))
            {
                return COMMUNICATION_PROTOCOL_TCP;
            }
            if (StringUtil.CompareIgnoreCase(endpoint.ListenUri.Scheme, ENDPOINT_SCHEME_NET_PIPE))
            {
                return COMMUNICATION_PROTOCOL_NET_PIPE;
            }
            throw new TisException("Not supported communication protocol : {0}", endpoint.ListenUri.Scheme);
        }

        public static string StringToScheme(this ServiceEndpoint endpoint, string communicationProtocol)
        {
            if (StringUtil.CompareIgnoreCase(communicationProtocol, COMMUNICATION_PROTOCOL_WS_HTTP))
            {
                return ENDPOINT_SCHEME_WS_HTTP;
            }

            if (StringUtil.CompareIgnoreCase(communicationProtocol, COMMUNICATION_PROTOCOL_HTTP))
            {
                return ENDPOINT_SCHEME_HTTP;
            }

            if (StringUtil.CompareIgnoreCase(communicationProtocol, COMMUNICATION_PROTOCOL_TCP))
            {
                return ENDPOINT_SCHEME_TCP;
            }
            if (StringUtil.CompareIgnoreCase(communicationProtocol, COMMUNICATION_PROTOCOL_NET_PIPE))
            {
                return ENDPOINT_SCHEME_NET_PIPE;
            }
            throw new TisException("Not supported communication protocol : {0}", communicationProtocol);
           }

    }

    #endregion
}
