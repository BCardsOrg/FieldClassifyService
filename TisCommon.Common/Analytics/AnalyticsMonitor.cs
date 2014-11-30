using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EQATEC.Analytics.Monitor;

namespace TiS.Core.TisCommon.Analytics
{
    public static class AnalyticsMonitor
    {
        private static AnalyticsMonitorHelper m_AnalyticsMonitorHelper;
        private static bool m_IsInitializeNotCalledLogged;

        public static void Initialize(ApplicationType applicationType)
        {
            if (m_AnalyticsMonitorHelper != null) { return; }

            m_AnalyticsMonitorHelper = new AnalyticsMonitorHelper(applicationType);
        }

        public static void SendLog(string logMessage)
        {
            if (!ValidateInitialize()) { return; }

            m_AnalyticsMonitorHelper.SendLog(logMessage);
        }

        public static void TrackFeature(string moduleName, string category, string featureName)
        {
            if (!ValidateInitialize()) { return; }

            m_AnalyticsMonitorHelper.TrackFeature(moduleName, category, featureName);      
        }

        public static void TrackFeatureValue(string moduleName, string category, string featureName, long trackedValue)
        {
            if (!ValidateInitialize()) { return; }

            m_AnalyticsMonitorHelper.TrackFeatureValue(moduleName, category, featureName, trackedValue);
        }

        public static void TrackFeatureStart(string moduleName, string category, string featureName)
        {
            if (!ValidateInitialize()) { return; }

            m_AnalyticsMonitorHelper.TrackFeatureStart(moduleName, category, featureName);
        }

        public static void TrackFeatureStop(string moduleName, string category, string featureName)
        {
            if (!ValidateInitialize()) { return; }

            m_AnalyticsMonitorHelper.TrackFeatureStop(moduleName, category, featureName);
        }

        public static void TrackFeatureCancel(string moduleName, string category, string featureName)
        {
            if (!ValidateInitialize()) { return; }

            m_AnalyticsMonitorHelper.TrackFeatureCancel(moduleName, category, featureName);
        }

        public static void SetInstallationInfo(string installationId, IDictionary<string, string> properties)
        {
            if (!ValidateInitialize()) { return; }

            m_AnalyticsMonitorHelper.SetInstallationInfo(installationId, properties);
        }

        public static void TrackException(Exception exception, string contextMessage)
        {
            if (!ValidateInitialize()) { return; }

            m_AnalyticsMonitorHelper.TrackException(exception, contextMessage);
        }

        private static bool ValidateInitialize()
        {
            if (m_AnalyticsMonitorHelper != null) { return true; }

            // If initialize was not called, we will write to the log (Only once) that the initialize was not called.
            if (!m_IsInitializeNotCalledLogged)
            {
                Log.WriteWarning("AnalyticsMonitor will not perform tracking, because Initialize was not called.");

                m_IsInitializeNotCalledLogged = true;
            }

            return false;
        }
    }
}
