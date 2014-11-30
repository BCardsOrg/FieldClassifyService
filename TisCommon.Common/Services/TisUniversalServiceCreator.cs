using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Services
{
    [ComVisible(false)]
    public class TisUniversalServiceCreator : TisServiceCreatorBase
    {
        public override object CreateService()
        {
            ITisServiceInfo serviceInfo = Context.ServicesHost.CheckedGetServiceInfo(
                Context.ApplicationName,
                Context.ServiceKey.ServiceName);

            string serviceImplType = serviceInfo.ServiceImplType;

            object service = ActivatorHelper.CreateInstance(
                serviceImplType,
                GetConstructorParams());

            OnPostServiceCreate(service);

            return service;
        }

        protected virtual object[] GetConstructorParams()
        {
            return EmptyArrays.ObjectArray;
        }

        protected virtual void OnPostServiceCreate(object oService)
        {
        }
    }
}
