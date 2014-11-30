using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using TiS.Core.TisCommon.Reflection;
using System.Collections.Generic;

namespace TiS.Core.TisCommon.Customizations
{
	public class ReflectionHelper
	{
		public static object CreateInstance (
			string sEventsDeclAssembly, 
			string sEventsDeclClassImpl)
		{
			ObjectHandle oWrappedObject = Activator.CreateInstance (sEventsDeclAssembly, sEventsDeclClassImpl);
			return oWrappedObject.Unwrap ();
		}

		public static object CreateInstance (Type oType)
		{
			return Activator.CreateInstance (oType);
		}

		public static EventInfo GetEventInfo (
			string sEventTargetType, 
			string sEventName)
		{
			Type oEventTargetType = Type.GetType (sEventTargetType);

			return GetEventInfo (oEventTargetType, sEventName);
		}

		public static EventInfo GetEventInfo (
			Type oEventTargetType, 
			string sEventName)
		{
			foreach (EventInfo oEventInfo in oEventTargetType.GetEvents (BindingFlags.Public))
			{
				if (oEventInfo.Name == sEventName )
				{
					return oEventInfo;
				}
			}

			return null;
		}

		public static string[] ExploreEvents (string sEventTargetType)
		{
			return ExploreEvents (Type.GetType (sEventTargetType));
		}

		public static string[] ExploreEvents (object oEventTarget)
		{
			return ExploreEvents (oEventTarget.GetType ());
		}

		public static string[] ExploreEvents (Type oEventTargetType)
		{
			ArrayList oEventNames = new ArrayList ();

			foreach (EventInfo oEventInfo in oEventTargetType.GetEvents (BindingFlags.Public))
			{
				oEventNames.Add (oEventInfo.Name);
			}

			return (string[]) oEventNames.ToArray (typeof (string));
		}

		public static EventInfo[] GetDeclaredEvents (object oEventSource)
		{
			if (oEventSource == null)
			{
				return new EventInfo[] {};
			}

			if (oEventSource is Type)
			{
				return GetDeclaredEvents (oEventSource as Type);
			}
			else
			{
				return GetDeclaredEvents (oEventSource.GetType ());
			}
		}

		public static EventInfo[] GetDeclaredEvents (Type oEventSourceType)
		{
            List<EventInfo> oEventsInfo = new List<EventInfo>();

            // Only services which are implementing ITisSupportEvents interface will be added to 
            // services events adapter
            if (oEventSourceType != null && typeof(ITisSupportEvents).IsAssignableFrom(oEventSourceType))
			{
				oEventsInfo.AddRange (oEventSourceType.GetEvents());

				ObtainAggregatedEvents(oEventSourceType, oEventsInfo, new List<Type>());
			}

			return oEventsInfo.ToArray();
		}

		public bool HasByRefParameter (MethodBase oMethodBase, out int[] ByRefParamIndices)
		{
			ArrayBuilder oParamIndices = new ArrayBuilder (typeof (int));

			if (oMethodBase != null)
			{
				ParameterInfo[] Params = oMethodBase.GetParameters();

				for (int i=0; i < Params.Length; i++)
				{
					ParameterInfo oParam = Params[i];

					if (oParam.IsOut || oParam.ParameterType.IsByRef)
					{
						oParamIndices.Add (i);
					}
				}
			}

			ByRefParamIndices = (int[]) oParamIndices.GetArray ();

			return ByRefParamIndices.Length > 0;
		}

        private static void ObtainAggregatedEvents(Type oEventSourceType, List<EventInfo> oEventsInfo, List<Type> oDisabledTypes)
		{
			if (oEventSourceType != null && oEventsInfo != null)
			{
				FieldInfo [] AttributedFields = 
					ReflectionUtil.GetFieldsWithAttribute (oEventSourceType, typeof(TisSupportedEventsAttribute));

				foreach(FieldInfo oField in AttributedFields)
				{
                    oDisabledTypes.AddRange(ObtainFieldDisabledTypes(oField));

                    if (!oDisabledTypes.Contains(oField.FieldType))
					{
						ObtainAggregatedEvents(oField.FieldType, oEventsInfo, oDisabledTypes);

                        EventInfo[] FieldEvents = oField.FieldType.GetEvents();

                        foreach (EventInfo oFieldEvent in FieldEvents)
                        {
                            bool bFound = false;

                            foreach (EventInfo oEventInvo in oEventsInfo)
                            {
                                bFound = StringUtil.CompareIgnoreCase(oFieldEvent.Name, oEventInvo.Name);

                                if (bFound)
                                {
                                    break;
                                }
                            }

                            if (!bFound)
                            {
                                oEventsInfo.Add(oFieldEvent);
                            }
                        }

                        //		oEventsInfo.AddRange (oField.FieldType.GetEvents());
					}
				}
			}
		}

		private static List<Type> ObtainFieldDisabledTypes (FieldInfo oField)
		{
			object[] Attributes = oField.GetCustomAttributes(typeof(TisSupportedEventsAttribute), false);

			if (Attributes.Length > 0)
			{
                return new List<Type>((Attributes[0] as TisSupportedEventsAttribute).DisabledTypes);
			}
			else
			{
				return new List<Type> ();
			}
		}
	}
}
