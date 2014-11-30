using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace TiS.Core.TisCommon.Analytics
{
    internal class AnalyticsMonitorHelper : IAnalyticsMonitor
    {
        private static object m_SyncObject = new object();

        private DateTime m_TrackedFeaturePerformedDateTime;

        // NOTE: We must hold the m_FeatureNameToTrackValueMap in ConcurrentDictionary, because it might be hold by static instance.
        private ConcurrentDictionary<string, long> m_FeatureNameToTrackValueMap = new ConcurrentDictionary<string, long>();

        private EQATEC.Analytics.Monitor.IAnalyticsMonitor m_Monitor;
        private ApplicationType m_ApplicationType;

        public AnalyticsMonitorHelper(ApplicationType applicationType)
        {
            m_TrackedFeaturePerformedDateTime = DateTime.Now;

            m_ApplicationType = applicationType;

            string productId = GetProductId(applicationType);
            m_Monitor = EQATEC.Analytics.Monitor.AnalyticsMonitorFactory.Create(productId);
            if (m_Monitor == null) { return; }

            m_Monitor.Start();
        }

        #region IAnalyticsMonitor Members

        public void TrackFeature(string moduleName, string category, string featureName)
        {
            if (m_Monitor == null) { return; }

            m_Monitor.TrackFeature(GetFeatureNameToTrack(m_ApplicationType,moduleName,category,featureName));
        }

        public void TrackFeatureValue(string moduleName, string category, string featureName, long trackedValue)
        {
			if (m_Monitor == null) { return; }

            string featureNameToTrack = GetFeatureNameToTrack(m_ApplicationType, moduleName, category, featureName);
            m_FeatureNameToTrackValueMap.AddOrUpdate(featureNameToTrack, trackedValue, (currentFeatureName, currentTrackedValue) => { return currentTrackedValue + trackedValue; });

            if (DateTime.Now > m_TrackedFeaturePerformedDateTime + TimeSpan.FromMinutes(10))
            {
                // We must lock this section, so ONLY the first thread getting here will actually track the features.
                lock (m_SyncObject)
                {
                    foreach (var keyValue in m_FeatureNameToTrackValueMap)
                    {
                        m_Monitor.TrackFeatureValue(keyValue.Key, keyValue.Value);
                    }

                    m_FeatureNameToTrackValueMap.Clear();
                }
            }
        }

        public void TrackFeatureStart(string moduleName, string category, string featureName)
        {
			if (m_Monitor == null) { return; }

            string featureNameToTrack = GetFeatureNameToTrack(m_ApplicationType, moduleName, category, featureName);
            m_Monitor.TrackFeatureStart(featureNameToTrack);
        }

        public void TrackFeatureStop(string moduleName, string category, string featureName)
        {
			if (m_Monitor == null) { return; }

            string featureNameToTrack = GetFeatureNameToTrack(m_ApplicationType, moduleName, category, featureName);
            m_Monitor.TrackFeatureStop(featureNameToTrack);
        }

        public void TrackFeatureCancel(string moduleName, string category, string featureName)
        {
			if (m_Monitor == null) { return; }

            string featureNameToTrack = GetFeatureNameToTrack(m_ApplicationType, moduleName, category, featureName);
            m_Monitor.TrackFeatureCancel(featureNameToTrack);
        }

        public void SendLog(string logMessage)
        {
			if (m_Monitor == null) { return; }

            m_Monitor.SendLog(logMessage);
        }

        public void SetInstallationInfo(string installationId, IDictionary<string, string> properties)
        {
			if (m_Monitor == null) { return; }

            m_Monitor.SetInstallationInfo(installationId, properties);
        }

        public void TrackException(Exception exception, string contextMessage)
        {
			if (m_Monitor == null) { return; }

            m_Monitor.TrackException(exception, contextMessage);
        }

        #endregion
        internal string GetFeatureNameToTrack(ApplicationType applicationType, string moduleName, string category, string featureName)
        {
            if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(featureName))
            {
                throw new TisException("moduleName and featureName cannot be empty.");
            }

            // NOTE: The featureName/moduleName/category will not be displayed if "." exists in them.
            // In order "." will be displayed, we must change it to ":".

            // If the module name is completion/controller, we will not writ the module name, because in this case the application type is also the module name.
            if (moduleName == ApplicationType.Completion.ToString() || moduleName == ApplicationType.Controller.ToString())
            {
                moduleName = string.Empty;
            }

            moduleName = moduleName.Replace(".", ":");
            category = category.Replace(".", ":");
            featureName = featureName.Replace(".", ":");

            string result = moduleName;
            if (!string.IsNullOrEmpty(category))
            {
                result += "_" + category;
            }

            result += "." + featureName;

            return result;
        }

        internal string GetProductId(ApplicationType applicationType)
        {
            switch (applicationType)
            {
                case ApplicationType.Completion:
                    {
                        return "6d64574861be4124a7d917742a695eb4";
                    }
                case ApplicationType.Controller:
                    {
                        return "700a0e54f4f3476182d302ff7a934cd7";
                    }
                case ApplicationType.Server:
                    {
                        return "9328A2407B44489EB1963678C5E55602";
                    }
                case ApplicationType.Station:
                    {
                        return "07cbe6931ec04574a6784227919552f4";
                    }
                case ApplicationType.Tool:
                    {
                        return "c889d225f94446de9e4782c67cd3907a";
                    }
            }

            return string.Empty;
        }
    }
}
