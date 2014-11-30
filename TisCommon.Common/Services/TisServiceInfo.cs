using System;
using System.Collections;
using System.Collections.Generic;
using TiS.Core.TisCommon.Security;
using TiS.Core.TisCommon.Reflection;
using TiS.Core.TisCommon.Customizations;
using TiS.Core.TisCommon.Validation;

namespace TiS.Core.TisCommon.Services
{
    #region TisServiceInfo

    public class TisServiceInfo : ITisServiceInfo, IPersistKeyProvider, ITisEventsProvider, ITisValidationProvider
    {
        private string m_sServiceCreatorType;
        private string m_sServiceName;
        private string m_sServiceImplType;
        private string[] m_NodeRoles;
        private string m_alias;
        private static object m_locker = new object();
        private TisEventParamsList<TisEventParams> m_supportedEventsList;
        private TisValidationMethodsList<TisValidationMethod> m_validationMethodsList;
        private TisStationDeclarationInfo m_stationDeclarationInfo;
        private bool m_bFromSchema;

        public TisServiceInfo()
        {
        }

        public TisServiceInfo(
            Type oServiceType,
            Type oCreatorType)
        {
            Init(
                GetServiceNameFromType(oServiceType),
                GetCreatorTypeString(oCreatorType),
                String.Empty,	// ImplType
                EmptyArrays.StringArray,  // Roles
                ServicesUsingMode.Free
                );
        }

        public TisServiceInfo(
            Type oServiceType,
            Type oCreatorType,
            string[] RequiredRoles)
        {
            Init(
                GetServiceNameFromType(oServiceType),
                GetCreatorTypeString(oCreatorType),
                String.Empty,	// ImplType
                RequiredRoles,
                ServicesUsingMode.Free
                );
        }

        public TisServiceInfo(
            Type oServiceType,
            Type oCreatorType,
            string[] RequiredRoles,
            Type oImplType)
        {
            Init(
                GetServiceNameFromType(oServiceType),
                GetCreatorTypeString(oCreatorType),
                GetImplTypeString(oImplType),
                RequiredRoles,
                ServicesUsingMode.Free
                );
        }

        public TisServiceInfo(
            string sServiceName,
            Type oCreatorType,
            string[] RequiredRoles,
            Type oImplType)
        {
            Init(
                sServiceName,
                GetCreatorTypeString(oCreatorType),
                GetImplTypeString(oImplType),
                RequiredRoles,
                ServicesUsingMode.Free
                );
        }

        public TisServiceInfo(
            string sServiceName,
            Type oCreatorType,
            string[] RequiredRoles,
            string sImplType)
        {
            Init(
                sServiceName,
                GetCreatorTypeString(oCreatorType),
                sImplType,
                RequiredRoles,
                ServicesUsingMode.Free
                );
        }

        public TisServiceInfo(
            string sServiceName,
            string sServiceCreatorType,
            string sServiceImplType,
            string[] RequiredRoles,
            ServicesUsingMode usingMode)
        {
            Init(
                sServiceName,
                sServiceCreatorType,
                sServiceImplType,
                RequiredRoles,
                usingMode);
        }

        public void SetFromSchema(bool bVal)
        {
            m_bFromSchema = bVal;
        }

        #region Implementation of ITisServiceInfo

        public string ServiceImplType
        {
            get
            {
                return m_sServiceImplType;
            }
        }

        public string ServiceCreatorType
        {
            get
            {
                return m_sServiceCreatorType;
            }
        }

        public string ServiceName
        {
            get
            {
                return m_sServiceName;
            }
        }

        public string[] RequiredRoles
        {
            get
            {
                return m_NodeRoles;
            }
        }

        public string[] SupportedPermissions
        {
            get
            {
                string[] SupportedPermissions = EmptyArrays.StringArray;

                if (ServiceImplType == String.Empty)
                {
                    return SupportedPermissions;
                }

                Type oServiceImplType = Type.GetType(ServiceImplType);

                if (oServiceImplType == null)
                {
                    return SupportedPermissions;
                }

                TisSupportedPermissionsAttribute oSupportedPermissionsAttribute =
                    (TisSupportedPermissionsAttribute)ReflectionUtil.GetAttribute(oServiceImplType, typeof(TisSupportedPermissionsAttribute));

                if (oSupportedPermissionsAttribute == null)
                {
                    return SupportedPermissions;
                }

                return oSupportedPermissionsAttribute.SupportedPermissions;
            }
        }


        public bool FromSchema
        {
            get { return m_bFromSchema; }
        }

        public ServicesUsingMode UsingMode { get; private set; }

        public List<TisEventParams> SupportedEvents
        {
            get
            {
                if (m_supportedEventsList == null)
                {
                    m_supportedEventsList = new TisEventParamsList<TisEventParams>(this);
                }

                return m_supportedEventsList;
            }
        }

        public List<TisValidationMethod> SupportedValidations
        {
            get
            {
                if (m_validationMethodsList == null)
                {
                    m_validationMethodsList =
                        new TisValidationMethodsList<TisValidationMethod>(new Validation.ITisValidationProvider[] { this });
                }

                return m_validationMethodsList;
            }
        }

        public TisStationDeclarationInfo StationDeclarationInfo
        {
            get
            {
                if (m_stationDeclarationInfo == null)
                {
                    m_stationDeclarationInfo = new TisStationDeclarationInfo(ServiceImplType);
                }

                return m_stationDeclarationInfo;
            }
        }

        public string Alias
        {
            get
            {
                if (!StringUtil.IsStringInitialized(m_alias))
                {
                    if (StringUtil.IsStringInitialized(ServiceImplType))
                    {
                        Type serviceImplType = Type.GetType(ServiceImplType);

                        if (serviceImplType != null)
                        {
                            TisServiceAliasAttribute serviceDescriptionAttribute =
                                (TisServiceAliasAttribute)ReflectionUtil.GetAttribute(serviceImplType, typeof(TisServiceAliasAttribute));

                            if (serviceDescriptionAttribute != null)
                            {
                                m_alias = serviceDescriptionAttribute.ServiceAlias;
                            }
                        }
                    }
                }

                return m_alias;
            }
        }

        #endregion

        #region ITisEventsProvider Members

        public object GetEventsProvider()
        {
            if (StringUtil.IsStringInitialized(ServiceImplType))
            {
                return Type.GetType(ServiceImplType);
            }

            return null;
        }

        #endregion

        #region ITisValidationProvider Members

        public Type ValidationProviderData
        {
            get
            {
                if (ServiceImplType != String.Empty)
                {
                    return Type.GetType(ServiceImplType);
                }

                return null;
            }
        }

        public Type ValidationSourceProviderData
        {
            get
            {
                return typeof(TisValidationsServiceSourceProvider);
            }
        }

        #endregion

        #region IPersistKeyProvider Members

        public string TypedPersistKey
        {
            get
            {
                return "Service$" + m_sServiceImplType;
            }
        }

        public string FullPersistKey
        {
            get
            {
                return "Service$" + m_sServiceImplType + "$" + m_sServiceName;
            }
        }

        #endregion

        private void Init(
            string sServiceName,
            string sServiceCreatorType,
            string sServiceImplType,
            string[] RequiredRoles,
            ServicesUsingMode usingMode)
        {
            lock (m_locker)
            {
                m_sServiceCreatorType = sServiceCreatorType;
                m_sServiceName = sServiceName;
                m_sServiceImplType = sServiceImplType;
                m_NodeRoles = RequiredRoles;
                UsingMode = usingMode;
            }
        }

        private static string GetCreatorTypeString(Type oCreatorType)
        {
            return GetFullTypeString(oCreatorType);
        }

        private static string GetImplTypeString(Type oImplType)
        {
            return GetFullTypeString(oImplType);
        }

        private static string GetServiceNameFromType(Type oServiceType)
        {
            return TisServicesUtil.GetShortTypeString(oServiceType);
        }

        private static string GetFullTypeString(Type oType)
        {
            string sType = String.Empty;

            if (oType != null)
            {
                sType = TisServicesUtil.GetFullTypeString(oType);
            }

            return sType;
        }
    }

    #endregion
}
