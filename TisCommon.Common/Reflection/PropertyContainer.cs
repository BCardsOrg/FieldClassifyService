using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace TiS.Core.TisCommon.Reflection
{
    // Applied to property setter method which fires notification event.
    [AttributeUsage(AttributeTargets.Method)]
    [ComVisible(false)]
    public class PropertySetterWithNotificationAttribute : Attribute
    {
    }

    #region PropertyContainer

    public class PropertyContainer
    {
        private static Dictionary<Type, PropertyContainer> m_propertyContainerCache;

        private FieldMemoryOffsetToNameMap m_fieldMemoryOffsetToNameMap;

        static PropertyContainer()
        {
            m_propertyContainerCache = new Dictionary<Type, PropertyContainer>();
        }

        public PropertyContainer(Type typeToMap)
        {
            MemberInfo[] Members = ReflectionUtil.GetMembersWithAttribute(
                typeToMap, 
                MemberTypes.Method, BindingFlags.Instance | BindingFlags.NonPublic, 
                typeof(PropertySetterWithNotificationAttribute));

            if (Members.Length == 1)
            {
                // Property setter method which fires notification event.
                MethodInfo propertySetterWithNotification = (MethodInfo)Members[0];

                m_fieldMemoryOffsetToNameMap = new FieldMemoryOffsetToNameMap(
                    typeToMap, 
                    propertySetterWithNotification);
            }
            else
            {
                Log.WriteWarning("Type [{0}] does not contain property setter method with notification event.", typeToMap);
            }
        }

        public static void MapType<T>()
        {
            if (!m_propertyContainerCache.ContainsKey(typeof(T)))
            {
                m_propertyContainerCache.Add(typeof(T), new PropertyContainer(typeof(T)));
            }
        }

        public static PropertyContainer ObtainPropertyContainer(Type mappedType)
        {
            PropertyContainer propertyContainer = null;

            m_propertyContainerCache.TryGetValue(mappedType, out propertyContainer);

            return propertyContainer;
        }

        public string GetPropertyName<T>(object fieldContainer, ref T field)
        {
            string propertyName = String.Empty;

            if (m_fieldMemoryOffsetToNameMap != null)
            {
                // Calculates "field" memory position within "fieldContainer" 
                // and looks up with this key for "field" name. 
                int fieldMemoryOffset = ILMemoryOffsetCalculator.GetOffset(fieldContainer, ref field);

                if (fieldMemoryOffset >= 0)
                {
                    try
                    {
                        propertyName = m_fieldMemoryOffsetToNameMap[fieldMemoryOffset];
                    }
                    catch (Exception exc)
                    {
                        Log.WriteError("Failed to obtain mapped property. Details : [{0}]", exc.Message);
                    }
                }
            }

            return propertyName;
        }
    }

    #endregion
}
