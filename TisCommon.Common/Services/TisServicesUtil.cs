using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Services
{
    #region ServicesUtil

    [ComVisible(false)]
    public class TisServicesUtil
    {
        private static Dictionary<string, Type> m_StringToType = new Dictionary<string, Type>();
        private static Dictionary<Type, string[]> m_TypeToImpTypes = new Dictionary<Type, string[]>();
        private static object m_locker = new object();

        public static void ValidateSystemApplication(string sAppName)
        {
            // Ensure that System application specified
            if (sAppName != TisServicesConst.SystemApplication)
            {
                throw new TisException(
                    "Application specified [{0}] " +
                    "is not System application",
                    sAppName);
            }
        }

        public static bool HasOneOfRequiredRoles(
            string[] PresentRoles,
            string[] RequiredRoles)
        {
            if (RequiredRoles.Length == 0)
            {
                // No roles required - ok
                return true;
            }

            return SetUtil<string>.GetIntersection(
                PresentRoles,
                RequiredRoles).Length > 0;
        }

        public static bool MustHaveOneOfRequiredRoles(
            string[] PresentRoles,
            string[] RequiredRoles)
        {
            if (RequiredRoles.Length > 0)
            {
                return SetUtil<string>.GetIntersection(
                    PresentRoles,
                    RequiredRoles).Length > 0;
            }

            return false;
        }

        public static bool ServicesHostNamesEqual(string sName1, string sName2)
        {
            return CompareIgnoreCase(sName1, sName2);
        }

        public static bool ApplicationNamesEqual(string sAppName1, string sAppName2)
        {
            return CompareIgnoreCase(sAppName1, sAppName2);
        }

        public static bool IsSystemApplication(string sAppName)
        {
            return ApplicationNamesEqual(
                sAppName,
                TisServicesConst.SystemApplication);
        }

        public static string GetShortTypeString(Type oType)
        {
            return ReflectionUtil.GetShortTypeString(oType);
        }

        public static string GetFullTypeString(Type oType)
        {
            return ReflectionUtil.GetFullTypeString(oType);
        }

        public static List<ITisServiceInfo> GetServicesInfoOfImplType(
            ITisServiceRegistry oServiceRegistry,
            Type oImplType)
        {
            List<ITisServiceInfo> servicesInfo = new List<ITisServiceInfo>();

            List<string> services = new List<string>(GetServicesOfImplType(oServiceRegistry, oImplType));

            services.ForEach(service => servicesInfo.Add(oServiceRegistry.GetInstalledServiceInfo(service)));

            return servicesInfo;
        }

        public static string[] GetServicesOfImplType(
            ITisServiceRegistry oServiceRegistry,
            Type oImplType)
        {
            string[] retArray;

            lock (m_locker)
            {
                if (m_TypeToImpTypes.TryGetValue(oImplType, out retArray) == true)
                    return retArray;

                List<string> oRetServices = new List<string>();

                foreach (ITisServiceInfo oServiceInfo in oServiceRegistry.InstalledServices)
                {
                    string sImplType = oServiceInfo.ServiceImplType;

                    if (string.IsNullOrEmpty(sImplType))
                    {
                        continue;
                    }

                    Type oServiceType;
                    if (m_StringToType.TryGetValue(sImplType, out oServiceType) == false)
                    {
                        try
                        {
                            oServiceType = Type.GetType(sImplType, false);
                        }
                        catch (Exception exc)
                        {
                            oServiceType = null;

                            Log.WriteWarning("Failed to load type {0}. Details : {1}", sImplType, exc.Message);
                        }

                        if (oServiceType != null)
                            m_StringToType.Add(sImplType, oServiceType);
                    }

                    if (oServiceType != null)
                    {
                        if (oImplType.IsAssignableFrom(oServiceType))
                        {
                            oRetServices.Add(oServiceInfo.ServiceName);
                        }
                    }
                }

                retArray = oRetServices.ToArray();

                m_TypeToImpTypes.Add(oImplType, retArray);
            }

            return retArray;
        }

        public static string ServiceNameFromServiceType(
            Type oServiceType)
        {
            return oServiceType.FullName;
        }
        private static bool CompareIgnoreCase(string s1, string s2)
        {
            return String.Compare(
                s1,
                s2,
                true // Ignore case
                ) == 0;
        }
    }

    #endregion
}
