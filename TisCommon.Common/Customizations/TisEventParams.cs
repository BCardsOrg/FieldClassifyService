using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Reflection;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

namespace TiS.Core.TisCommon.Customizations
{
	public class TisEventParams : ITisEventParams
	{
		private EventInfo m_oEventInfo;
        private ITisMethodSignature m_oMethodSignature;

		public TisEventParams (EventInfo oEventInfo)
		{
			m_oEventInfo = oEventInfo;

			m_oMethodSignature = new MethodSignature (m_oEventInfo.EventHandlerType);
		}

		#region ITisEventParams Members

		public string Name
		{
			get
			{
				return m_oEventInfo.Name;
			}
		}

		public Type EventHandlerType
		{
			get
			{
				return m_oEventInfo.EventHandlerType;
			}
		}

		public ITisMethodSignature MethodSignature
		{
			get
			{
				return m_oMethodSignature;
			}
		}

		public void AddEventHandler (object oTarget, Delegate oDelegate)
		{
			string sEventName = oDelegate.Method.Name;

            oTarget = EnsureEventTarget (oTarget, sEventName);

			if (oTarget != null)
			{
				m_oEventInfo.AddEventHandler (oTarget, oDelegate);
			}
			else
			{
				Log.WriteWarning("No target object found for event {0} [{1}]", sEventName, oTarget.GetType().Name);
			}
		}

        public void RemoveEventHandler(object oTarget, Delegate oDelegate)
        {
            string sEventName = oDelegate.Method.Name;

            oTarget = EnsureEventTarget(oTarget, sEventName);

            if (oTarget != null)
            {
                m_oEventInfo.RemoveEventHandler(oTarget, oDelegate);
            }
            else
            {
                Log.WriteWarning("No target object found for event {0} [{1}]", sEventName, oTarget.GetType().Name);
            }
        }

        internal void RemoveEventHandler(object oTarget, FieldInfo eventFieldInfo)
        {
            oTarget = EnsureEventTarget(oTarget, Name);

            if (oTarget != null)
            {
                ReflectionUtil.SetFieldValue(oTarget, eventFieldInfo, null);
            }
            else
            {
                Log.WriteWarning("No target object found for event {0} [{1}]", Name, oTarget.GetType().Name);
            }
        }

        #endregion

        private object EnsureEventTarget(object oTarget, string sEventName)
		{
			object oFoundTarget = null;

			if (oTarget != null)
			{
				Type oTargetType = oTarget.GetType();

				if (VerifyEvent (oTargetType, sEventName))
				{
					oFoundTarget = oTarget;
				}
				else
				{
					FieldInfo [] AttributedFields = 
						ReflectionUtil.GetFieldsWithAttribute (oTargetType, typeof(TisSupportedEventsAttribute));

					foreach(FieldInfo oField in AttributedFields)
					{
						oFoundTarget = EnsureEventTarget(ReflectionUtil.GetFieldValue(oTarget, oField), sEventName);

						if (oFoundTarget != null)
						{
							break;
						}
					}
				}
			}

			return oFoundTarget;
		}

		private bool VerifyEvent(Type oType, string sEventName)
		{
			if (oType == null || !StringUtil.IsStringInitialized(sEventName))
			{
				return false;
			}

			EventInfo oEvent = oType.GetEvent (sEventName);

			return oEvent != null ? true : false;
		}
	}

	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class MethodSignature : ITisMethodSignature
	{
        [DataMember]
		private MethodReturn m_methodReturn;
        [DataMember]
        private List<MethodParam> m_methodParams = new List<MethodParam>();

        public MethodSignature()
        {
        }

		public MethodSignature (MethodParam[] Params, Type oReturnType)
		{
			m_methodReturn = new MethodReturn (oReturnType);

			m_methodParams.AddRange (Params);
		}

		public MethodSignature (Type oEventHandlerDelegate)
		{
			MethodInfo oInvoke = oEventHandlerDelegate.GetMethod ("Invoke");

			m_methodReturn = new MethodReturn (oInvoke);

    		ParameterInfo[] Params = oInvoke.GetParameters ();

			foreach (ParameterInfo oParam in Params)
			{
				m_methodParams.Add (new MethodParam (oParam));
			}
		}

        public MethodSignature(ParameterInfo[] Params, Type oReturnType)
        {
            m_methodReturn = new MethodReturn(oReturnType);

            foreach (ParameterInfo oParam in Params)
            {
                m_methodParams.Add(new MethodParam(oParam));
            }
        }


		#region ITisMethodSignature Members

        public MethodParam[] Params
		{
			get
			{
				return  m_methodParams.ToArray ();
			}
		}

		public string[] ParamNames
		{
			get
			{
				ArrayBuilder oArrayBuilder = new ArrayBuilder (typeof (string));

				foreach (ITisMethodParam oParam in m_methodParams)
				{
					oArrayBuilder.Add (oParam.ParamName);
				}

				return (string[]) oArrayBuilder.GetArray ();
			}
		}

		public Type[] ParamTypes
		{
			get
			{
				ArrayBuilder oArrayBuilder = new ArrayBuilder (typeof (Type));

				foreach (ITisMethodParam oParam in m_methodParams)
				{
					oArrayBuilder.Add (oParam.ParamType);
				}

				return (Type[]) oArrayBuilder.GetArray ();
			}
		}

		public MethodReturn ReturnInfo
		{
			get
			{
				return m_methodReturn;
			}
		}
		#endregion

	}

	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class MethodReturn : MarshalInfo, ITisMethodReturn
	{
        [DataMember]
		private Type m_returnType;

        public MethodReturn()
        {
        }

		public MethodReturn (Type oReturnType)
		{
			m_returnType = oReturnType;
		}

		public MethodReturn (MethodInfo oInvokeMethod) : 
			base ((EmbedAttributeAttribute) ReflectionUtil.GetAttribute (oInvokeMethod.ReflectedType, typeof (EmbedAttributeAttribute)))
		{
			m_returnType = oInvokeMethod.ReturnType;
		}

		#region ITisMethodReturn Members

		public Type ReturnType
		{
			get
			{
				return m_returnType;
			}
		}

		#endregion
	}


	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class MethodParam : MarshalInfo, ITisMethodParam
	{
        [DataMember]
		private string m_sParamName;
        [DataMember]
        private Type m_oParamType;
        [DataMember]
        private bool m_bIsOut;

        public MethodParam()
        {
        }

		public MethodParam (string sParamName, Type oParamType, bool bIsOut)
		{
			m_sParamName = sParamName;
			m_oParamType = oParamType;
			m_bIsOut     = bIsOut;
		}

		public MethodParam (ParameterInfo oParam) :
			base ((EmbedAttributeAttribute) ReflectionUtil.GetAttribute (oParam, typeof (EmbedAttributeAttribute)))
		{
			m_sParamName = oParam.Name;
			m_oParamType = oParam.ParameterType;
			m_bIsOut     = oParam.IsOut | oParam.ParameterType.IsByRef;
		}

		#region ITisMethodParam Members

		public string ParamName
		{
			get
			{
				return m_sParamName;
			}
		}

		public Type ParamType
		{
			get
			{
				return m_oParamType;
			}
		}

		public bool IsOut
		{
			get
			{
				return m_bIsOut;
			}
		}

		#endregion

	}

	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class MarshalInfo
	{
        [DataMember]
		private bool m_bHasMarshal;
        [DataMember]
		private UnmanagedType m_enMarshalType;

		public MarshalInfo ()
		{
		}

		public MarshalInfo (EmbedAttributeAttribute oMarshalInfo)
		{
			ParseMarshalInfo (oMarshalInfo);
		}

		public bool HasMarshal
		{
			get
			{
				return m_bHasMarshal;
			}
		}

		public UnmanagedType MarshalType
		{
			get
			{
				return m_enMarshalType;
			}
		}

		private void ParseMarshalInfo (EmbedAttributeAttribute oMarshalInfo)
		{
			m_bHasMarshal = 
				(oMarshalInfo != null) && 
				StringUtil.CompareIgnoreCase (oMarshalInfo.AttributeName, "MarshalAs");

			if (m_bHasMarshal)
			{
				string[] MarshalTypeNameParts = oMarshalInfo.Arguments [0].Value.Split (new char[] {'.'});

				string sMarshalTypeName = MarshalTypeNameParts [MarshalTypeNameParts.Length - 1];

				m_enMarshalType = (UnmanagedType) Enum.Parse  (typeof (UnmanagedType), sMarshalTypeName);
			}
		}
	}

    public class TisEventParamsList<T> : List<TisEventParams> where T : TisEventParams
	{
        private List<string> m_eventNames = new List<string>();

		public TisEventParamsList (object oEventSource, string eventName = null)
		{
			object oEventsProvider = GetFinalEventsProvider(oEventSource);

            FillContainer(oEventsProvider, eventName);
		}

		public TisEventParamsList (Type oEventSourceType)
		{
			FillContainer (oEventSourceType);
		}

        public TisEventParams this[string eventName]
        {
            get
            {
                IEnumerable<TisEventParams> foundEventParams =
                    from eventParams in this
                    where StringUtil.CompareIgnoreCase(eventParams.Name, eventName)
                    select eventParams;

                if (foundEventParams.Count() == 1)
                {
                    return foundEventParams.ToArray()[0];
                }

                return null;
            }
        }

        public List<string> EventNames
        {
            get
            {
                return m_eventNames;
            }
        }

        static ConcurrentDictionary<object, IEnumerable<EventInfo>> m_allEventsInfo = new ConcurrentDictionary<object, IEnumerable<EventInfo>>();
        static IEnumerable<EventInfo> GetEventsInfo(object eventsProvider)
        {
            return m_allEventsInfo.GetOrAdd(eventsProvider, key => ReflectionHelper.GetDeclaredEvents(key));
        }

        private void FillContainer(object eventsProvider, string eventName = null)
		{
			if (eventsProvider != null)
			{
                IEnumerable<EventInfo> eventsInfo = GetEventsInfo(eventsProvider);

                if ( string.IsNullOrEmpty(eventName) == false)
                {
                    eventsInfo = eventsInfo.Where(x => x.Name == eventName);
                }

				foreach (EventInfo eventInfo in eventsInfo)
                {
                    IEnumerable<TisEventParams> foundEventParams =
                        from eventParams in this
                        where StringUtil.CompareIgnoreCase(eventParams.Name, eventInfo.Name)
                        select eventParams;

                    if (foundEventParams.Count() == 0)
                    {
                        this.Add(new TisEventParams(eventInfo));
                        m_eventNames.Add(eventInfo.Name);
                    }
                }
			}
		}

		private object GetFinalEventsProvider(object eventSource)
		{
			if (eventSource == null)
			{
				return null;
			}

			object oEventsProvider = eventSource;

			while (oEventsProvider is ITisEventsProvider)
			{
				oEventsProvider = GetFinalEventsProvider ((oEventsProvider as ITisEventsProvider).GetEventsProvider());
			}

			return oEventsProvider;
		}
    }
}
