
namespace TiS.Core.TisCommon.Services.Web
{
    #region TisWebServiceInfo

    public class TisWebServiceInfo : TisServiceInfo, ITisWebServiceInfo
    {
        public TisWebServiceInfo(
            string name,
            string contractType,
            string creatorType,
            string implementationType,
            string[] requiredRoles,
            bool isInternal,
            bool isAutoStart,
            bool isSingleton,
            string configurationName,
            ServicesUsingMode usingMode)
            : base(name, creatorType, implementationType, requiredRoles, usingMode)
        {
            WebServiceContractType = contractType;
            WebServiceConfigurationName = configurationName;
            WebServiceIsAutoStart = isAutoStart;
            WebServiceIsInternal = isInternal;
            WebServiceIsSingleton = isSingleton;
        }

        #region ITisWebServiceInfo Members

        public string WebServiceContractType { get; set; }
        public string WebServiceConfigurationName { get; set; }
        public bool WebServiceIsAutoStart { get; set; }
        public bool WebServiceIsInternal { get; set; }
        public bool WebServiceIsSingleton { get; set; }

        #endregion
    }

    #endregion
}
