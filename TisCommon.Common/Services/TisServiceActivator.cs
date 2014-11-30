using System;

namespace TiS.Core.TisCommon.Services
{
    internal class TisServiceActivator : ITisServiceProvider
    {
        private string m_sAppName;
        private TisServicesHost m_oServicesHost;

        private MRUCache m_oCreatorsCache = new MRUCache();

        //
        //	Public methods
        //

        public TisServiceActivator(
            string sAppName,
            TisServicesHost oServicesHost)
        {
            m_sAppName = sAppName;
            m_oServicesHost = oServicesHost;

            m_oCreatorsCache.OnCacheMiss +=
                new CacheMissEvent(CreatorsCache_OnCacheMiss);
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_oCreatorsCache.OnCacheMiss -=
                new CacheMissEvent(CreatorsCache_OnCacheMiss);

            if (m_oCreatorsCache != null)
            {
                m_oCreatorsCache.Dispose();
            }
        }

        #endregion

        #region ITisServiceProvider implementation

        public object GetService(
            TisServiceKey oServiceKey)
        {
            object oService = null;

            try
            {
                // Obtain service creator
                ITisServiceCreator oCreator = GetCreator(oServiceKey);

                // Create the service instance using creator
                oService = oCreator.CreateService();
            }
            catch (Exception oExc)
            {
                Log.WriteError(
                    "ServiceActivator.GetService({0}) Failed, {1}",
                    oServiceKey,
                    oExc.Message);

                throw;
            }


            // Return the service
            return oService;
        }

        public void ReleaseService(
            TisServiceKey oServiceKey,
            ITisServiceInfo serviceInfo = null,
            object oService = null)
        {
            if (oService == null && IsInstantiated(oServiceKey))
            {
                oService = m_oCreatorsCache.Get(oServiceKey);
            }

            if (oService != null)
            {
                // Obtain service creator
                ITisServiceCreator oCreator = GetCreator(oServiceKey);

                oCreator.ReleaseService(oService);
            }
        }

        public void AddService(
            TisServiceKey oServiceKey,
            object oService)
        {
        }

        public bool IsInstantiated(TisServiceKey oServiceKey)
        {
            return m_oCreatorsCache.IsInCache(oServiceKey);
        }

        #endregion

        private ITisServiceCreator GetCreator(TisServiceKey oServiceKey)
        {
            return (ITisServiceCreator)m_oCreatorsCache.Get(oServiceKey);
        }

        private object CreatorsCache_OnCacheMiss(
            object oSender,
            CacheMissEventArgs oArgs)
        {
            TisServiceKey oServiceKey = (TisServiceKey)oArgs.Key;

            // Obtain service info
            ITisServiceInfo oServiceInfo =
                m_oServicesHost.CheckedGetServiceInfo(m_sAppName, oServiceKey.ServiceName);

            // Validate we can host the service
            ValidateCanHostService(oServiceInfo);

            // Obtain service creator
            ITisServiceCreator oCreator =
                InstanciateCreator(oServiceInfo);

            // Set creator context
            SetCreatorContext(oCreator, oServiceKey);

            return oCreator;
        }

        private void SetCreatorContext(
            ITisServiceCreator oCreator,
            TisServiceKey oServiceKey)
        {
            // Set creator context
            m_oServicesHost.CreatorContextSetter.SetCreatorContext( 
                oCreator,
                m_sAppName,
                oServiceKey,
                m_oServicesHost);
        }

        private ITisServiceCreator InstanciateCreator(
            ITisServiceInfo oServiceInfo)
        {
            // Get creator type
            string sCreatorType =
                oServiceInfo.ServiceCreatorType;

            // Create Creator instance
            object oObjCreator = ActivatorHelper.CreateInstance(
                sCreatorType,
                EmptyArrays.ObjectArray);

            ITisServiceCreator oCreator = null;

            try
            {
                // Try to cast 
                oCreator = (ITisServiceCreator)oObjCreator;
            }
            catch (InvalidCastException oExc)
            {
                throw new TisException(
                    oExc, // Inner
                    "Object [{0}] doesn't implements ITisServiceCreator",
                    oObjCreator);
            }

            return oCreator;
        }

        private ITisServiceRegistry ServiceRegistry
        {
            get { return m_oServicesHost.GetServiceRegistry(m_sAppName); }
        }

        private void ValidateCanHostService(ITisServiceInfo oServiceInfo)
        {
            if (!m_oServicesHost.CanHostService(m_sAppName, oServiceInfo))
            {
                throw new TisException(
                    "Can't activate service [{0}] " +
                    "because the ServicesHost [{1}] doesn't have required roles " +
                    "([{2}])",
                    oServiceInfo.ServiceName,
                    m_oServicesHost.Name,
                    StringUtil.ToCommaDelimitedString(oServiceInfo.RequiredRoles));
            }
        }
    }
}
