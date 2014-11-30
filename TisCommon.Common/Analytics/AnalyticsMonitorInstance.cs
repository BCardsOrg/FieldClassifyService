using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon.Analytics
{
    internal class AnalyticsMonitorInstance : IAnalyticsMonitor
    {
        private AnalyticsMonitorHelper m_AnalyticsMonitorHelper;
        private bool m_enableEquatechStatistics;
        public AnalyticsMonitorInstance(ApplicationType applicationType)
        {
            var config = new BasicConfiguration();
            m_enableEquatechStatistics = config.EnableEquatechStatistics;
            if (m_enableEquatechStatistics)
                m_AnalyticsMonitorHelper = new AnalyticsMonitorHelper(applicationType);
        }

        #region IAnalyticsMonitor Members

        public void SendLog(string logMessage)
        {
            if (!ValidateInitialize()) { return; }
            m_AnalyticsMonitorHelper.SendLog(logMessage);
        }

        public void TrackFeature(string moduleName, string category, string featureName)
        {
            if (!ValidateInitialize()) { return; }
            m_AnalyticsMonitorHelper.TrackFeature(moduleName, category, featureName);
        }

        public void TrackFeatureValue(string moduleName, string category, string featureName, long trackedValue)
        {
            if (!ValidateInitialize()) { return; }
            m_AnalyticsMonitorHelper.TrackFeatureValue(moduleName, category, featureName, trackedValue);
        }

        public void TrackFeatureStart(string moduleName, string category, string featureName)
        {
            if (!ValidateInitialize()) { return; }
            m_AnalyticsMonitorHelper.TrackFeatureStart(moduleName, category, featureName);
        }

        public void TrackFeatureStop(string moduleName, string category, string featureName)
        {
            if (!ValidateInitialize()) { return; }
            m_AnalyticsMonitorHelper.TrackFeatureStop(moduleName, category, featureName);
        }

        public void TrackFeatureCancel(string moduleName, string category, string featureName)
        {
            if (!ValidateInitialize()) { return; }
            m_AnalyticsMonitorHelper.TrackFeatureCancel(moduleName, category, featureName);
        }

        public void SetInstallationInfo(string installationId, IDictionary<string, string> properties)
        {
            if (!ValidateInitialize()) { return; }
            m_AnalyticsMonitorHelper.SetInstallationInfo(installationId, properties);
        }

        public void TrackException(Exception exception, string contextMessage)
        {
            if (!ValidateInitialize()) { return; }
            m_AnalyticsMonitorHelper.TrackException(exception, contextMessage);
        }

        #endregion

        private bool ValidateInitialize()
        {
            if (m_AnalyticsMonitorHelper != null) { return true; }

            // If Equatech Statistics disabled, we will write to the log (Only once) that Equatech Statistics disabled.
            if (!m_enableEquatechStatistics)
            {
                Log.WriteWarning("AnalyticsMonitor will not perform tracking, because Equatech Statistics disabled");
                m_enableEquatechStatistics = true;
            }

            return false;
        }
    }
}
