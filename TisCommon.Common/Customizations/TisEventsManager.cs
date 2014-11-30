using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Customizations.MethodInvokers.Managed;
using TiS.Core.TisCommon.Storage;
using TiS.Core.TisCommon.Storage.ObjectStorage;
using TiS.Core.TisCommon.Services;
using TiS.Core.TisCommon.Configuration;
using System.Runtime.Serialization;
using TiS.Core.TisCommon.Transactions;
using System.Collections.Generic;
using TiS.Core.TisCommon.DataModel;
using TiS.Core.TisCommon.FilePath;

namespace TiS.Core.TisCommon.Customizations
{
	[ComVisible (false)]
    public class TisEventsManager : ITisEventsManager, ITisEventBindings, ITisEventBindingsLegacy, ITransactable
	{
		private const string EVENTS_PERSIST_NAME = "Events.xml"; 

		private ITransactionalStorage     m_oApplicationResources;
		private ITisObjectStorage         m_eventsStorage;
		private TisEventBindingsMngr      m_oEventBindingsMngr;
		private TisInvokeTypesMngr        m_oInvokeTypesMngr;
        private TisTransactionManager     m_oTransactionManager;
        private string                    m_sApplicationName;
        private ICustomAssemblyResolver   m_oCustomAssemblyResolver;
        private DictionaryWithEvents<string, ITisInvokeType> m_invokeTypes;

		bool m_bDebugMode;
		bool m_bDisableEvents;
		private bool m_bLoaded;


		public TisEventsManager (
            ITransactionalStorage oApplicationResources, 
			string sApplicationName,
            CustomAssemblyResolver customAssemblyResolver,
            DictionaryWithEvents<string, ITisInvokeType> invokeTypes)
		{
            m_sApplicationName = sApplicationName;
            m_oApplicationResources = oApplicationResources;
            m_oCustomAssemblyResolver = customAssemblyResolver;
            m_invokeTypes = invokeTypes;

            DataContractSerializer eventsDataContractSerializer = new DataContractSerializer(typeof(TisEventBindingsMngr), new Type[] { typeof(EventBindingInfoMngr) });

            m_eventsStorage = new ObjectStorage(
                m_oApplicationResources,
                new ObjectReadDelegate(eventsDataContractSerializer.ReadObject),
                new ObjectWriteDelegate(eventsDataContractSerializer.WriteObject));

            InitTransactionManager(sApplicationName);
        }

		#region ITisEventsManager Members

        public event EventHandler OnDisposing;

        public string ApplicationName
        {
            get
            {
                return m_sApplicationName;
            }
        }

		public bool DebugMode
		{
			get
			{
				return m_bDebugMode;
			}
			set
			{
				m_bDebugMode = value;

                foreach (ITisInvokeType oInvokeType in InvokeTypesMngr.All)
                {
                    ITisEventInvoker oEventInvoker = oInvokeType.GetEventsInvoker();

                    oEventInvoker.DebugMode = m_bDebugMode;
                }


			}
		}

		public bool DisableEvents
		{
			get
			{
				return m_bDisableEvents;
			}
			set
			{
				m_bDisableEvents = value;
			}
		}

		public ITisEventBindings EventBindings
		{
			get
			{
				LoadIfNeeded ();

				return this as ITisEventBindings;
			}
		}

		public ITisEventBindingsLegacy EventBindingsLegacy
		{
			get
			{
				LoadIfNeeded ();

				return this as ITisEventBindingsLegacy;
			}
		}

		public ITransactable EventsTransaction
		{
			get
			{
                return this;
			}
		}

		public ITisEventParams[] GetSupportedEvents (object oEventSource)
		{
            TisEventParamsList<TisEventParams> oEventContainer = new TisEventParamsList<TisEventParams>(oEventSource);

			return oEventContainer.ToArray ();
		}

		public IList GetSupportedEventNames (object oEventSource)
		{
            TisEventParamsList<TisEventParams> oEventContainer = new TisEventParamsList<TisEventParams>(oEventSource);

			return oEventContainer.EventNames;
		}

		public IList GetSupportedEventNames (Type oEventSourceType)
		{
            TisEventParamsList<TisEventParams> oEventContainer = new TisEventParamsList<TisEventParams>(oEventSourceType);

			return oEventContainer.EventNames;
		}

		public IList GetSupportedEventNames (string sEventSourceTypeName)
		{
			Type oEventSourceType = null;

			if (sEventSourceTypeName != String.Empty)
			  oEventSourceType = Type.GetType (sEventSourceTypeName);

			return GetSupportedEventNames (oEventSourceType);
		}

		public IList GetInstalledInvokeTypeNames()
		{
            return InvokeTypesMngr.InvokeTypeNames;
		}

		public ITisEventParams GetEvent (object oEventSource, string sEventName)
		{
            TisEventParamsList<TisEventParams> oEventContainer = new TisEventParamsList<TisEventParams>(oEventSource, sEventName);

			return oEventContainer [sEventName];
		}

        public ITisInvokeParams[] GetMatchingMethodsBySignature(string sFileName, ITisMethodSignature oMethodSignature)
		{
            //m_oCustomAssemblyResolver.CustomizationDir = CustomizationDir;

            using (TisEventsExplorer oEventsExplorer = new TisEventsExplorer(m_oCustomAssemblyResolver))
            {
                return oEventsExplorer.GetMatchingMethodsBySignature(sFileName, oMethodSignature);
            }
		}

        public string[] GetReferencedAssemblies(string sFileName)
		{
            //m_oCustomAssemblyResolver.CustomizationDir = CustomizationDir;

            using (TisEventsExplorer oEventsExplorer = new TisEventsExplorer(m_oCustomAssemblyResolver))
            {
                return oEventsExplorer.GetReferencedAssemblies(sFileName);
            }
		}
        
        public ITisInvokeParams[] GetMatchingMethodsByEvent(string sFileName, object oEventSource, string sEventName)
        {
            ITisEventParams oEvent = GetEvent(oEventSource, sEventName);

            return GetMatchingMethodsBySignature(sFileName, oEvent.MethodSignature);
        }

        public object FireEvent(object oEventSource, string sEventName, ref object[] InOutParams)
        {
            ITisEventParams oEventParams = GetEvent(oEventSource, sEventName);

            return FireEvent(oEventSource, oEventParams, ref InOutParams);
        }

        public object FireEvent(object oEventSource, ITisEventParams oEventParams, ref object[] InOutParams)
		{
            if (oEventParams != null && !DisableEvents)
            {
                ITisInvokeParams oInvokeParams;

                ITisEventInvoker oEventInvoker =
                    GetEventsInvoker(oEventSource, oEventParams.Name, out oInvokeParams);

                if (oEventInvoker != null)
                {
                    return oEventInvoker.Invoke(oInvokeParams, oEventParams, ref InOutParams);
                }
            }
            else
            {
                if (DisableEvents)
                {
                    Log.WriteWarning("Events are disabled.");
                }
            }

            return null;
		}

        internal object FireEvent(object oEventSource, object oEventBindingKey, string sEventName, ref object[] InOutParams)
        {
            ITisEventParams oEventParams = GetEvent(oEventSource, sEventName);

            return FireEvent(oEventBindingKey, oEventParams, ref InOutParams);
        }

        private ITisEventInvoker GetEventsInvoker(object oEventSource, string sEventName, out ITisInvokeParams oInvokeParams)
		{
			oInvokeParams = GetBinding (oEventSource, sEventName);

			if (oInvokeParams != null)
			{
				ITisInvokeType oInvokeType =
                    InvokeTypesMngr.GetInvokeType(oInvokeParams.InvokeType);

				if (oInvokeType != null)
				{
					return oInvokeType.GetEventsInvoker ();
				}
			}

			return null;
		}

		public event TisGetInvokerDelegate OnGetInvoker;

		#endregion

        #region ITisInternalEventsManager

        public string CustomizationDir { get; set; }

        public IList GetInstalledInvokeTypeDescriptions()
        {
            ArrayList oInvokeTypeDescriptions = new ArrayList();

            foreach (ITisInvokeType oInvokeType in InvokeTypesMngr.All)
            {
                oInvokeTypeDescriptions.Add(oInvokeType.Miscellaneous.Description);
            }

            return oInvokeTypeDescriptions;
        }

        public event TisEventBindingDelegate OnBindingAction;

		#endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (OnDisposing != null)
            {
                OnDisposing(this, new EventArgs());
            }

            if (m_oInvokeTypesMngr != null)
            {
                m_oInvokeTypesMngr.Dispose();
                m_oInvokeTypesMngr = null;
            }

            if (m_oTransactionManager != null)
            {
                m_oTransactionManager.OnExecuteTransaction -= new TransactionManagerEvent(OnCommitTransaction);
                m_oTransactionManager.OnRollbackTransaction -= new TransactionManagerEvent(OnRollbackTransaction);

                m_oTransactionManager = null;
            }
        }

        #endregion

        public TisEventBindingsMngr EventBindingsMngr
		{
			get
			{
				LoadIfNeeded ();

				return m_oEventBindingsMngr;
			}
		}

        public TisInvokeTypesMngr InvokeTypesMngr
        {
            get
            {
                if (m_oInvokeTypesMngr == null)
                {
                    m_oInvokeTypesMngr = new TisInvokeTypesMngr(
                        m_invokeTypes,
                        new TisGetInvokerDelegate(OnGetInvokerHandler));
                }

                LoadIfNeeded();

                return m_oInvokeTypesMngr;
            }
        }

        private void LoadIfNeeded()
		{
			if (!m_bLoaded)
			{
				Load ();
			}
		}

		private void InitTransactionManager (string sAppName)
		{
            m_oTransactionManager = new TisTransactionManager();

			m_oTransactionManager.OnExecuteTransaction   += new TransactionManagerEvent (OnCommitTransaction);
			m_oTransactionManager.OnRollbackTransaction += new TransactionManagerEvent (OnRollbackTransaction);

			m_oTransactionManager.AddTransactionMember (m_oApplicationResources);
		}

		private void OnCommitTransaction(object sender, EventArgs oArgs)
		{
            m_eventsStorage.StoreObject(EventBindingsMngr, EVENTS_PERSIST_NAME);
		}

		private void OnRollbackTransaction(object sender, EventArgs oArgs)
		{
			Load ();
		}

		private void Load ()
		{
			try
			{
				m_oEventBindingsMngr = 
					(TisEventBindingsMngr) m_eventsStorage.LoadObject (EVENTS_PERSIST_NAME);

				OnBindingsDeserialization ();
			}
			catch(Exception oExc)
			{
				Log.Write(
					Log.Severity.INFO,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					oExc.Message
					);


				m_oEventBindingsMngr = 
                    new TisEventBindingsMngr (new TisEventBindingDelegate (OnBindingActionHandler));
			}

			m_bLoaded = true;
		}


		#region ITisEventBindings Members

		public ITisEventBinding[] All
		{
			get
			{
				return EventBindingsMngr.All;
			}
		}

		public int Count
		{
			get
			{
				return All.Length;
			}
		}

		public void Clear()
		{
			if (m_bLoaded)
			{
				m_oEventBindingsMngr.Clear ();
			}

			m_bLoaded = false;
		}

		public ITisEventBinding AddDNBinding (
			object oEventSource, 
			string sEventName, 
			string sModuleName,
			string sClassName,
			string sMethodName)
		{
            ITisEventBinding eventBinding = EventBindingsMngr.AddBinding(
				oEventSource, 
				sEventName, 
				new TisDNInvokeParams (sModuleName, sClassName, sMethodName));

            UpdateEventSource(oEventSource, eventBinding, true);

            return eventBinding;
        }

		public ITisEventBinding AddBinding(object oEventSource, string sEventName, ITisInvokeParams oInvokeParams)
		{
            ITisEventBinding eventBinding = EventBindingsMngr.AddBinding(oEventSource, sEventName, oInvokeParams);

            UpdateEventSource(oEventSource, eventBinding, true);

            return eventBinding;
        }

		public ITisEventBinding AddBinding(object oEventSource, string sEventName, MethodInfo oMethodInfo)
		{
            ITisEventBinding eventBinding = EventBindingsMngr.AddBinding(
                oEventSource, 
                sEventName, 
                oMethodInfo);

            UpdateEventSource(oEventSource, eventBinding, true);

            return eventBinding;
		}

		public ITisEventBinding AddBinding (object oEventSource, string sEventName, TisInvokeType enInvokeType, string sEventString)
		{
            ITisEventBinding eventBinding = null;

			switch (enInvokeType)
			{
				case TisInvokeType.DOTNET :
				{
                    eventBinding = EventBindingsMngr.AddBinding(
						oEventSource, 
						sEventName, 
						new TisDNInvokeParams (sEventString));
                    break;
                }
			}

            UpdateEventSource(oEventSource, eventBinding, true);

            return eventBinding;
        }

		public ITisEventBinding AddBinding (object oEventSource, string sEventName, string sInvokeTypeName, string sEventString)
		{
            ITisEventBinding eventBinding = EventBindingsMngr.AddBinding(
				oEventSource, 
				sEventName, 
				new TisInvokeParams (sInvokeTypeName, sEventString));

            UpdateEventSource(oEventSource, eventBinding, true);

            return eventBinding;
		}

        public ITisEventBinding AddVBABinding(
            object oEventSource,
            string sEventName,
            string sClassName,
            string sMethodName)
        {
            return null;
        }

        public ITisEventBinding AddWin32DLLBinding(
            object oEventSource,
            string sEventName,
            string sModuleName,
            string sMethodName)
        {
            return null;
        }

        public void RemoveBinding(object oEventSource, string sEventName)
        {
            ITisEventBinding eventBinding = 
                EventBindingsMngr.RemoveBinding(oEventSource, sEventName);

            UpdateEventSource(oEventSource, eventBinding, false);
        }

        private void UpdateEventSource(
            object oEventSource, 
            ITisEventBinding eventBinding,
            bool bindingAdded)
        {
            if (eventBinding != null && oEventSource is TisDataLayerTreeNode)
            {
                List<EventAssemblyInfo> eventSourceBoundEventsAssembliesContainer =
                    ((TisDataLayerTreeNode)oEventSource).BoundEventsAssemblies;

                EventAssemblyInfo eventSourceEventAssemblyInfo = FindEventAssemblyInfo(
                    eventSourceBoundEventsAssembliesContainer, 
                    eventBinding.InvokeParams.ModuleName);

                TisDataLayerTreeNode flowset = (TisDataLayerTreeNode)((TisDataLayerTreeNode)oEventSource).Root;

                List<EventAssemblyInfo> flowsetBoundEventsAssembliesContainer =
                    flowset.BoundEventsAssemblies;

                EventAssemblyInfo flowsetEventAssemblyInfo = FindEventAssemblyInfo(
                    flowsetBoundEventsAssembliesContainer,
                    eventBinding.InvokeParams.ModuleName);

                if (bindingAdded)
                {
                    if (StringUtil.IsStringInitialized(((TisInvokeParams)eventBinding.InvokeParams).ModulePath))
                    {
                        EventAssemblyInfo eventAssemblyInfo = new EventAssemblyInfo(
                                    ((TisInvokeParams)eventBinding.InvokeParams).ModulePath,
                                    eventBinding.InvokeParams.ModuleName,
                                    this,
                                    m_oCustomAssemblyResolver.CustomizationDir);

                        if (eventSourceEventAssemblyInfo == null)
                        {
                            eventSourceBoundEventsAssembliesContainer.Add(eventAssemblyInfo);
                        }

                        if (flowsetEventAssemblyInfo == null)
                        {
                            flowsetBoundEventsAssembliesContainer.Add(eventAssemblyInfo);
                        }
                    }
                }
                else
                {
                    if (eventSourceEventAssemblyInfo != null)
                    {
                        eventSourceBoundEventsAssembliesContainer.Remove(eventSourceEventAssemblyInfo);
                    }

                    if (flowsetEventAssemblyInfo != null)
                    {
                        flowsetBoundEventsAssembliesContainer.Remove(flowsetEventAssemblyInfo);
                    }
                }
            }
        }

        private EventAssemblyInfo FindEventAssemblyInfo(
            List<EventAssemblyInfo> boundEventsAssembliesContainer,
            string moduleName)
        {
            foreach (EventAssemblyInfo eventAssemblyInfo in boundEventsAssembliesContainer)
            {
                if (StringUtil.CompareIgnoreCase(eventAssemblyInfo.AssemblyName, moduleName))
                {
                    return eventAssemblyInfo;
                }
            }

            return null;
        }

		public bool Contains (object oEventSource)
		{
			return EventBindingsMngr.Contains (oEventSource);
		}

		public bool Contains (object oEventSource, string sEventName)
		{
			return EventBindingsMngr.Contains (oEventSource, sEventName);
		}

		public ITisInvokeParams GetBinding(object oEventSource, string sEventName)
		{
			ITisEventBinding oEventBinding = EventBindingsMngr.GetBinding (oEventSource, sEventName);

			if (oEventBinding == null)
			{
				return null;
			}

			return oEventBinding.InvokeParams;
		}

        public ITisEventBinding[] GetBindings(object oEventSource)
        {
            List<ITisEventBinding> bindings = EventBindingsMngr.GetBindings(oEventSource);

            if (bindings != null)
            {
                return EventBindingsMngr.GetBindings(oEventSource).ToArray();
            }
            else
            {
                return null;
            }
        }

        #endregion

		#region ITisEventBindingsLegacy Members

		ITisEventBinding ITisEventBindingsLegacy.AddBinding(
			object oEventSource, 
			string sEventName, 
			string sEventString)
		{
			string[] EventStringParts = sEventString.Split (new char[] {':'});

			string sModuleName = EventStringParts[0];
			string sClassName;
			string sMethodName;

			if (sEventString.StartsWith ("+"))
			{
				sClassName   = EventStringParts[1];
				sMethodName  = EventStringParts[2];

				return AddDNBinding (
					oEventSource,
					sEventName,
					sModuleName.TrimStart (new char[] {'+'}),
					sClassName,
					sMethodName);
			}

			return null;
		}

		#endregion

		private void OnGetInvokerHandler(string sTypeName, TisInvokeTypeMiscellaneous oMiscellaneous)
		{
			if (OnGetInvoker != null)
			{
				OnGetInvoker (sTypeName, oMiscellaneous);
			}
        }

		private void OnBindingsDeserialization ()
		{
			m_oEventBindingsMngr.OnBindingAction += new TisEventBindingDelegate (OnBindingActionHandler); 
		}

		private void OnBindingActionHandler (ref string oEventSourceBindingKey)
		{
			if (OnBindingAction != null)
			{
				OnBindingAction (ref oEventSourceBindingKey);
			}
		}

        #region ITransactable Members

        public void PrepareTransaction()
        {
            m_oTransactionManager.PrepareTransaction();
        }

        public void ExecuteTransaction()
        {
            m_oTransactionManager.ExecuteTransaction();
        }

        public void RollbackTransaction()
        {
            m_oTransactionManager.RollbackTransaction();
        }

        public bool InTransaction
        {
            get
            {
                return m_oTransactionManager.InTransaction;
            }
        }

        #endregion
    }
}
