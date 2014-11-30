using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections.Generic;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Customizations
{
	[ComVisible (false)]
	public delegate void TisEventBindingDelegate (ref string oEventSourceBindingKey);

    #region TisEventBindingsMngr

	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisEventBindingsMngr
    {
        [DataMember]
        private DictionaryWithEvents<string, EventBindingInfoMngr> m_oBindings = new DictionaryWithEvents<string, EventBindingInfoMngr>();

		public TisEventBindingsMngr()
		{
		}

		public TisEventBindingsMngr(TisEventBindingDelegate oBindingActionDelegate) : base ()
		{
			OnBindingAction += oBindingActionDelegate;
		}

		#region ITisEventBindingsMngr Members

		public event TisEventBindingDelegate OnBindingAction;

		public ITisEventBinding[] All
		{
			get
			{
				ArrayList oArrayList = new ArrayList ();

				foreach (EventBindingInfoMngr oBindingInfoMngr in m_oBindings.Values)
				{
					oArrayList.AddRange (oBindingInfoMngr.All);
				}

				ITisEventBinding[] _Bindings = (ITisEventBinding[]) Array.CreateInstance (typeof (ITisEventBinding), oArrayList.Count);

				oArrayList.CopyTo (_Bindings);

				return _Bindings;
			}
		}

		public void Clear ()
		{
			m_oBindings.Clear ();
		}

        public ITisEventBinding AddBinding(object oEventSource, string sEventName, ITisInvokeParams oInvokeParams)
		{
			string oPersistKey = ObtainEventSourceBindingKey (oEventSource);

			EventBindingInfoMngr oBindingInfoMngr = ObtainBindingInfoMngr (oPersistKey);

			if (oBindingInfoMngr == null)
			{
				oBindingInfoMngr = new EventBindingInfoMngr ();
			}

			ITisEventBinding oEventBinding = oBindingInfoMngr.Add (sEventName, oInvokeParams);

			m_oBindings.Add (oPersistKey, oBindingInfoMngr);

			return oEventBinding; 
		}

		public ITisEventBinding AddBinding(object oEventSource, string sEventName, MethodInfo oMethodInfo)
		{
			return AddBinding (oEventSource, sEventName, new TisDNInvokeParams (oMethodInfo));
		}

		public void RemoveBindings(object oEventSource)
		{
			string oPersistKey = ObtainEventSourceBindingKey (oEventSource);

			m_oBindings.Remove(oPersistKey);
		}

		public void RemoveBinding(object oEventSource, string[] EventsName)
		{
			foreach (string sEventName in EventsName)
			{
				RemoveBinding (oEventSource, sEventName);
			}
		}

        public ITisEventBinding RemoveBinding(object oEventSource, string sEventName)
		{
			string oPersistKey = ObtainEventSourceBindingKey (oEventSource);

			return InternalRemoveBinding(oPersistKey, sEventName);
		}

		public List<ITisEventBinding> GetBindings (object oEventSource)
		{
			string oPersistKey = ObtainEventSourceBindingKey (oEventSource);

			EventBindingInfoMngr oBindingInfoMngr = ObtainBindingInfoMngr (oPersistKey);

			if (oBindingInfoMngr != null)
			{
				return oBindingInfoMngr.All;
			}

			return null;
		}

		public ITisEventBinding GetBinding(object oEventSource, string sEventName)
		{
            string oPersistKey = ObtainEventSourceBindingKey(oEventSource);

			EventBindingInfoMngr oBindingInfoMngr = ObtainBindingInfoMngr (oPersistKey);

			if (oBindingInfoMngr != null)
			{
				return oBindingInfoMngr.GetByEventName (sEventName);
			}

			return null;
		}

		public bool Contains (object oEventSource)
		{
            string oPersistKey = ObtainEventSourceBindingKey(oEventSource);

			EventBindingInfoMngr oBindingInfoMngr = ObtainBindingInfoMngr (oPersistKey);

			return oBindingInfoMngr == null ? false : true;
		}

		public bool Contains (object oEventSource, string sEventName)
		{
            string oPersistKey = ObtainEventSourceBindingKey(oEventSource);

			EventBindingInfoMngr oBindingInfoMngr = ObtainBindingInfoMngr (oPersistKey);

			if (oBindingInfoMngr != null)
			{
				return oBindingInfoMngr.Contains (sEventName);
			}

			return false;
		}

		public bool Contains (object oEventSource, ITisEventBinding oEventBindingInfo)
		{
            string oPersistKey = ObtainEventSourceBindingKey(oEventSource);

			EventBindingInfoMngr oBindingInfoMngr = ObtainBindingInfoMngr (oPersistKey);

			if (oBindingInfoMngr != null)
			{
				return oBindingInfoMngr.Contains(oEventBindingInfo);
			}

			return false;
		}

		public virtual string GetPersistKey (object oEventSource)
		{
            string persistKey = default(string);

			if (oEventSource != null)
			{
                if (oEventSource is IPersistKeyProvider)
                {
                    persistKey = (string)(oEventSource as IPersistKeyProvider).FullPersistKey;
                }
                else
                {
                    if (oEventSource is string)
                    {
                        persistKey = (string)oEventSource;
                    }
                    else
                    {
                        throw new TisException("Events source [{0}] should implement IPersistKeyProvider interface.", oEventSource.GetType().ToString());
                    }
                }
			}

            return persistKey;
		}

        #endregion

		private string ObtainEventSourceBindingKey (object oEventSource)
		{
			string oPersistKey = GetPersistKey (oEventSource);

			if (OnBindingAction != null)
			{
				OnBindingAction (ref oPersistKey);
			}

			if (oPersistKey == null)
			{
				throw new TisException ("Failed to obtain persistence key for [{0}]", oEventSource.ToString ());
			}

			return oPersistKey;
		}

		private EventBindingInfoMngr ObtainBindingInfoMngr (string oPersistKey)
		{
			return (EventBindingInfoMngr) m_oBindings[oPersistKey];
		}

        private ITisEventBinding InternalRemoveBinding(string oPersistKey, string sEventName)
		{
			EventBindingInfoMngr oBindingInfoMngr = null;

			if (m_oBindings.ContainsKey(oPersistKey))
			{
				oBindingInfoMngr = (EventBindingInfoMngr) m_oBindings [oPersistKey];
			}
			else
			{
				oBindingInfoMngr = new EventBindingInfoMngr ();
			}

            ITisEventBinding eventBinding = oBindingInfoMngr.GetByEventName(sEventName);

			oBindingInfoMngr.Remove (new string[] {sEventName});

            return eventBinding;
		}
	}

    #endregion

    #region EventBindingInfoMngr

	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    [KnownType(typeof(EventBindingInfo))]
    public class EventBindingInfoMngr
	{
        [DataMember]
        private DictionaryWithEvents<string, ITisEventBinding> m_oEventsBindingInfo = new DictionaryWithEvents<string, ITisEventBinding>();

		#region IEventBindingInfoMngr Members

		public List<ITisEventBinding> All
		{
			get
			{
                return new List<ITisEventBinding>(m_oEventsBindingInfo.Values);
			}
		}

		public ITisEventBinding Add (string sEventName, ITisInvokeParams oInvokeParams)
		{
			ITisEventBinding oEventBindingInfo = GetByEventName (sEventName);
 
			if (oEventBindingInfo == null)
			{
				oEventBindingInfo = new EventBindingInfo (sEventName, oInvokeParams);

				m_oEventsBindingInfo.Add (sEventName, oEventBindingInfo);
			}

			return oEventBindingInfo;
		}

		public void Clear()
		{
			m_oEventsBindingInfo.Clear ();
		}

		public void Remove(string[] EventsName)
		{
			foreach (string sEventName in EventsName)
			{
				m_oEventsBindingInfo.Remove (sEventName);
			}
		}

		void Remove(ITisEventBinding[] EventsBindingInfo)
		{
			foreach (ITisEventBinding oEventBinding in EventsBindingInfo)
			{
				m_oEventsBindingInfo.Remove (oEventBinding.EventName);
			}
		}

		public ITisEventBinding GetByEventName(string sEventName)
		{
			return (ITisEventBinding) m_oEventsBindingInfo [sEventName];
		}

		public bool Contains(string sEventName)
		{
			return m_oEventsBindingInfo.ContainsKey(sEventName);
		}

        public bool Contains(ITisEventBinding oEventBindingInfo)
		{
			return m_oEventsBindingInfo.ContainsKey(oEventBindingInfo.EventName);
		}

		#endregion

	}
	#endregion

	#region EventBindingInfo

	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    [KnownType(typeof(TisInvokeParams))]
    [KnownType(typeof(TisDNInvokeParams))]
    [KnownType(typeof(TisWin32DLLInvokeParams))]
    public class EventBindingInfo : ITisEventBinding
	{
		public EventBindingInfo ()
		{
		}

		public EventBindingInfo (string sEventName, ITisInvokeParams oInvokeParams)
		{
			EventName    = sEventName;
			InvokeParams = oInvokeParams;
		}

		#region ITisEventBinding Members

        [DataMember]
		public string EventName {get; set;}

        [DataMember]
        public ITisInvokeParams InvokeParams { get; set; }

		#endregion
	}

	#endregion

}
