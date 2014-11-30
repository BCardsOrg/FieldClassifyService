using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
//using System.Linq;

namespace TiS.Core.TisCommon.Analytics
{
    public static class AnalyticsMonitorAccess
    {
        private static ConcurrentDictionary<ApplicationType, IAnalyticsMonitor> m_ApplicationTypeAnalyticsMonitor = new ConcurrentDictionary<ApplicationType, IAnalyticsMonitor>();

        public static IAnalyticsMonitor Get(ApplicationType applicationType)
        {
            var result = m_ApplicationTypeAnalyticsMonitor.GetOrAdd(applicationType, applType => new AnalyticsMonitorInstance(applType));
            return result;
        }
    }
}
