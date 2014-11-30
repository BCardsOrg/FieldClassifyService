
namespace TiS.Core.TisCommon.Services.Web
{
    #region ITisWebServiceInfo

    public interface ITisWebServiceInfo : ITisServiceInfo
    {
        string WebServiceContractType { get; }
        string WebServiceConfigurationName { get; }
        bool WebServiceIsAutoStart { get; }
        bool WebServiceIsInternal { get; }
        bool WebServiceIsSingleton { get; }
    }

    #endregion
}
