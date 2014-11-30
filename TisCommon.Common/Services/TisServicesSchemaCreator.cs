using System;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Services.Web;

namespace TiS.Core.TisCommon.Services
{
	[ComVisible(false)]
	public class TisServicesSchemaCreator
	{
		protected static ITisServiceInfo CreateServiceInfo(
			Type oServiceType,
			Type oCreatorType)
		{
			return new TisServiceInfo(oServiceType, oCreatorType);
		}

		protected static ITisServiceInfo CreateServiceInfo(
			Type	 oServiceType,
			Type	 oCreatorType,
			string[] RequiredRoles)		
		{
			return new TisServiceInfo(oServiceType, oCreatorType, RequiredRoles);
		}

		protected static ITisServiceInfo CreateServiceInfo(
			Type     oServiceType,
			Type     oCreatorType,
			string[] RequiredRoles,
			Type     oImplType)
		{
			return new TisServiceInfo(oServiceType, oCreatorType, RequiredRoles, oImplType);
		}

		protected static ITisServiceInfo CreateServiceInfo(
			string   sServiceName,
			Type     oCreatorType,
			string[] RequiredRoles,
			Type     oImplType)
		{
			return new TisServiceInfo(sServiceName, oCreatorType, RequiredRoles, oImplType);
		}

		protected static ITisServiceInfo CreateServiceInfo(
			string   sServiceName,
			Type     oCreatorType,
			string[] RequiredRoles)
		{
			return new TisServiceInfo(
				sServiceName, 
				oCreatorType, 
				RequiredRoles,
				String.Empty);
		}

		protected static ITisServiceInfo CreateServiceInfo(
			string   sServiceName,
			Type     oCreatorType,
			string[] RequiredRoles,
			string   sImplType)
		{
			return new TisServiceInfo(
				sServiceName, 
				oCreatorType, 
				RequiredRoles, 
				sImplType);
		}

        protected static ITisServiceInfo CreateServiceInfo(
            string serviceName,
            string creatorType,
            string[] requiredRoles,
            string implType,
            ServicesUsingMode usingMode = ServicesUsingMode.Free)
        {
            return new TisServiceInfo(
                serviceName,
                creatorType,
                implType,
                requiredRoles,
                usingMode);
        }

        protected static ITisServiceInfo CreateWebServiceInfo(
            string name,
            string contractType,
            string creatorType,
            string[] requiredRoles,
            string implementationType,
            bool isInternal = false,
            bool isAutoStart = false,
            bool isSingleton = false,
            string configurationName = null,
            ServicesUsingMode usingMode = ServicesUsingMode.Free)
        {
            return new TisWebServiceInfo(
                 name,
                 contractType,
                 creatorType,
                 implementationType,
                 requiredRoles,
                 isInternal,
                 isAutoStart,
                 isSingleton,
                 configurationName,
                 usingMode);
        }
    }
}
