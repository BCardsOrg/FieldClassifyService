using System;
using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Activities;
using TiS.Core.TisCommon.Customizations.MethodInvokers.Managed;

namespace TiS.Core.TisCommon.Reflection
{
	public class ReflectionUtil
	{
        private static AssemblyName MsCorLib = Assembly.GetAssembly(typeof(AppDomain)).GetName();
        private static AssemblyName SystemDrawing = Assembly.GetAssembly(typeof(Bitmap)).GetName();
        private static AssemblyName SystemActivity = Assembly.GetAssembly(typeof(Activity)).GetName();

        private static AssemblyName[] m_MSAssemblies =
            new AssemblyName[] { MsCorLib, SystemDrawing, SystemActivity };

        private static AssemblyName[] m_SystemAssemblies =
            new AssemblyName[] { TisAssembly, MsCorLib, SystemDrawing, SystemActivity };

        private static Dictionary<Type, ILReflector> m_allILReflectors =
            new Dictionary<Type, ILReflector>();

        private static Dictionary<MethodInfo, InvokeMethodDelegate> m_methodsCache =
            new Dictionary<MethodInfo, InvokeMethodDelegate>();

        private static Dictionary<ICustomAttributeProvider, Dictionary<Type, Attribute[]>> m_attributeProviderToTypeMap =
            new Dictionary<ICustomAttributeProvider, Dictionary<Type, Attribute[]>>();

        private static object m_locker = new object();

        public static PropertyInfo GetMandatoryProperty(
			Type oType,
			string sPropertyName)
		{
            PropertyInfo oPropertyInfo = null;

			// Get property info
            try
            {
                oPropertyInfo =
                    oType.GetProperty(sPropertyName);
            }
            catch
            {
            }
			
			return oPropertyInfo;
		}

		public static object GetPropertyValue(
			object oObj,
			string sPropertyName)
		{
			PropertyInfo oPropertyInfo =
                GetMandatoryProperty(oObj.GetType(), sPropertyName);

            return GetPropertyValue(oObj, oPropertyInfo);
		}

        public static object SetPropertyValue(
            object oObj,
            string sPropertyName,
            object oVal)
        {
            PropertyInfo oPropertyInfo =
                GetMandatoryProperty(oObj.GetType(), sPropertyName);

            return SetPropertyValue(oObj, oPropertyInfo, oVal);
        }

        public static PropertyInfo[] GetPublicProperties(Type oType)
		{
			return GetPublicProperties(oType, null, null);
		}

        public static MemberInfo[] GetMembersWithAttribute(
            Type oType,
            MemberTypes memberType,
            BindingFlags bindingAttr,
            Type oAttributeType)
        {
            return GetMembers(
                oType,
                memberType,
                bindingAttr,
                new MemberFilter(AttributeMemberFilter),
                oAttributeType);
        }

        public static PropertyInfo[] GetPublicPropertiesWithAttribute(
			Type oType,
			Type oAttributeType)
		{
			return GetPublicProperties(
				oType,
				new MemberFilter(AttributeMemberFilter),
				oAttributeType);
		}

		public static Attribute GetAttribute(
			ICustomAttributeProvider oAttributeProvider,
			Type					 oAttributeType)
		{
            Attribute[] attributes = GetAttributes(oAttributeProvider, oAttributeType);

            Attribute oRetVal = null;

            if (attributes.Length > 0)
			{
				// Found, take the first
                oRetVal = (Attribute)attributes[0];
			}

			return oRetVal;
		}

		public static Attribute[] GetAttributes(
			ICustomAttributeProvider oAttributeProvider,
			Type					 oAttributeType)
		{
            lock (m_locker)
            {
                Attribute[] attributes = null;

                Dictionary<Type, Attribute[]> typeToAttributesMap = null;

                m_attributeProviderToTypeMap.TryGetValue(oAttributeProvider, out typeToAttributesMap);

                if (typeToAttributesMap != null)
                {
                    typeToAttributesMap.TryGetValue(oAttributeType, out attributes);
                }
                else
                {
                    typeToAttributesMap = new Dictionary<Type, Attribute[]>();
                    m_attributeProviderToTypeMap[oAttributeProvider] = typeToAttributesMap;
                }

                if (attributes == null)
                {
                    // Search for attributes of specified type
                    object[] Attributes =
                        oAttributeProvider.GetCustomAttributes(oAttributeType, true);

                    ArrayBuilder oAttributes = new ArrayBuilder(Attributes);

                    attributes = (Attribute[])oAttributes.GetArray(typeof(Attribute));

                    typeToAttributesMap[oAttributeType] = attributes;
                }

                return attributes;
            }
		}


		public static bool HasAttribute(
			ICustomAttributeProvider oAttributeProvider,
			Type					 oAttributeType)
		{
			return (GetAttribute(oAttributeProvider, oAttributeType) != null);
		}

        public static MemberInfo[] GetMembers(
            Type oType,
            MemberTypes memberType,
            BindingFlags bindingAttr,
            MemberFilter oFilter,
            object oFilterParams)
        {
            // Query for members
            MemberInfo[] Members = oType.FindMembers(
                memberType,
                bindingAttr,
                oFilter, 
                oFilterParams);

            // Create array of MemberInfo
            MemberInfo[] RetVal =
                new MemberInfo[Members.Length];

            // Fill array
            Members.CopyTo(RetVal, 0);

            return RetVal;
        }

        public static PropertyInfo[] GetPublicProperties(
			Type		 oType,
			MemberFilter oFilter,
			object       oFilterParams)
		{
			// Query for public properties
			MemberInfo[] Members = oType.FindMembers(
				MemberTypes.Property, 
				BindingFlags.Public | BindingFlags.Instance,
				oFilter, oFilterParams);
			
			// Create array of PropertyInfo
			PropertyInfo[] RetVal = 
				new PropertyInfo[Members.Length];

			// Fill array
			Members.CopyTo(RetVal, 0);

			return RetVal;
		}

		public static PropertyInfo GetPublicProperty(
			Type		 oType,
			MemberFilter oFilter,
			object       oFilterParams)
		{
			// Query properties
			PropertyInfo[] Properties = GetPublicProperties(
				oType,
				oFilter,
				oFilterParams);
			
			if(Properties.Length == 0)
			{
				// None 
				return null;
			}
			
			// Must be maximum 1 
			if(Properties.Length > 1)
			{
				throw new TisException(
					"More than 1 property of Type [{0}] " +
					"passed filter [{1}, {2}]", 
					oType,
					oFilter,
					oFilterParams);
			}
			
			// Return the one that passed
			return Properties[0];
		}

		public static FieldInfo[] GetFields(
			Type		 oType,
			MemberFilter oFilter,
			object       oFilterParams)
		{
			// Query for public fields
			MemberInfo[] Members = oType.FindMembers(
				MemberTypes.Field, 
				BindingFlags.NonPublic | BindingFlags.Instance,
				oFilter, oFilterParams);
			
			// Create array of PropertyInfo
			FieldInfo[] RetVal = 
				new FieldInfo[Members.Length];

			// Fill array
			Members.CopyTo(RetVal, 0);

			return RetVal;
		}

		public static FieldInfo[] GetPublicFields(Type		 oType)
		{
			// Query for public fields
			return oType.GetFields(BindingFlags.Public | BindingFlags.Instance);
		}


		public static FieldInfo[] GetFieldsWithAttribute(
			Type oType,
			Type oAttributeType)
		{
			return GetFields(
				oType,
				new MemberFilter(AttributeMemberFilter),
				oAttributeType);
		}

		public static FieldInfo GetField(
			Type		 oType,
			MemberFilter oFilter,
			object       oFilterParams)
		{
			// Query Fields
			FieldInfo[] Fields = GetFields(
				oType,
				oFilter,
				oFilterParams);
			
			if(Fields.Length == 0)
			{
				// None 
				return null;
			}
			
			// Must be maximum 1 
			if(Fields.Length > 1)
			{
				throw new TisException(
					"More than 1 Field of Type [{0}] " +
					"passed filter [{1}, {2}]", 
					oType,
					oFilter,
					oFilterParams);
			}
			
			// Return the one that passed
			return Fields[0];
		}

		public static bool AttributeMemberFilter(
			MemberInfo oMember,
			object oRawAttributeType)
		{
			bool bRetVal = false;

			Type oAttributeType = (Type)oRawAttributeType;

			if( HasAttribute(oMember, oAttributeType) )
			{
				// Found
				bRetVal = true;
			}

			return bRetVal;
		}

		public static bool IsCompoundType(Type oType)
		{
			bool bRetVal = false;

			// If class (but not string) or interface
			if( (oType.IsClass && 
				!(oType == typeof(string))) || 
				oType.IsInterface)
			{
				bRetVal = true;
			}
			
			return bRetVal;
		}

		public static bool IsPrimitiveType(Type oType)
		{
			if(oType.IsPrimitive || 
				(oType == typeof(string)))
			{
				return true;
			}

			return false;
		}

		public static bool PrimitivePropertyFilter(
			MemberInfo	  oMember, 
			object		  oNull)
		{
			return IsPrimitiveType(
				((PropertyInfo)oMember).PropertyType);
		}

		public static bool CompoundPropertyFilter(
			MemberInfo	  oMember, 
			object		  oNull)
		{
			return IsCompoundType(
				((PropertyInfo)oMember).PropertyType);
		}

		public static string GetShortTypeString(Type oType)
		{
			return oType.FullName;
		}

		public static string GetFullTypeString(Type oType)
		{
			return oType.FullName + ", " + oType.Assembly.GetName().Name;		
		}

		public static Enum ToEnum(Type oEnumType, int nEnVal)
		{
			return (Enum)Enum.Parse(oEnumType,
				Enum.GetName(oEnumType, nEnVal),
				false);		
		}

		public static Array GetStaticFieldValuesOfType(Type oType, Type oFieldType)
		{
			ArrayBuilder oFieldValues = new ArrayBuilder(oFieldType);

			FieldInfo[] Fields = oType.GetFields(
				BindingFlags.Public | 
				BindingFlags.Static | 
				BindingFlags.FlattenHierarchy
				);

            ILReflector ILReflector = GetILReflector(oType);

			foreach(FieldInfo oField in Fields)
			{
                object oFieldVal = ILReflector.GetFieldValue(null, oField);

				if(oFieldVal == null)
				{
					continue;
				}

				if(oFieldType.IsAssignableFrom(oFieldVal.GetType()))
				{
					oFieldValues.Add(oFieldVal);
				}
			}

			return oFieldValues.GetArray();
		}

		public static string GetFullMethodName(System.Reflection.MethodBase oMethod)
		{
			string sTypeName   = oMethod.DeclaringType.Name;
			string sMethodName = oMethod.Name;
			string sAsmName    = oMethod.DeclaringType.Assembly.GetName().Name;

			return String.Format("[{0}]{1}.{2}", sAsmName, sTypeName, sMethodName);
		}


        public static void GetAssemblyLocation(
            Assembly oRootAssembly,
            ArrayBuilder oAssemblyList,
            AssemblyName[] SystemAssemblies,
            bool bCustomOnly,
            bool bWithDependencies,
            bool includeRootAssembly = true,
            CustomAssemblyResolver assemblyResolver=null)
        {
            if (assemblyResolver != null)
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(assemblyResolver.AssemblyResolveHandler);
            }

            try
            {
                if (!bCustomOnly || IsCustomAssembly(
                                      SystemAssemblies,
                                      oRootAssembly.GetName()))
                {
                    if (includeRootAssembly)
                    {
                        oAssemblyList.AddIfNotExists(oRootAssembly.Location);
                    }

                    if (bWithDependencies)
                    {
                        AssemblyName[] Dependencies = oRootAssembly.GetReferencedAssemblies();

                        foreach (AssemblyName oAssemblyName in Dependencies)
                        {
                            try
                            {
                                if (!bCustomOnly || IsCustomAssembly(
                                                      SystemAssemblies,
                                                      oAssemblyName))
                                {
                                    Assembly oDependentAssembly = Assembly.Load(oAssemblyName);

                                    // Check for circullar references
                                    if (!oAssemblyList.Contains(oDependentAssembly.Location))
                                    {
                                        GetAssemblyLocation(oDependentAssembly, oAssemblyList, SystemAssemblies, bCustomOnly, bWithDependencies, true, assemblyResolver);
                                    }
                                }
                            }
                            catch (Exception oExc)
                            {
                                Log.WriteException(oExc);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (assemblyResolver != null)
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(assemblyResolver.AssemblyResolveHandler);
                }
            }
        }

		public static bool IsCustomAssembly(AssemblyName[] SystemAssemblies, AssemblyName oAssembly)
		{
			if (SystemAssemblies != null)
			{
				foreach(AssemblyName oSystemAssembly in SystemAssemblies)
				{
					if (!IsCustomAssembly(oSystemAssembly, oAssembly))
					{
						return false;
					}
				}
			}
           
			return true;
		}

		public static bool IsCustomAssembly(AssemblyName oSystemAssembly, AssemblyName oAssembly)
		{
			byte[] SystemPKToken = oSystemAssembly.GetPublicKeyToken();

			byte[] AssemblyPKToken = oAssembly.GetPublicKeyToken();

			bool bCustomAssembly = true;

			if (SystemPKToken != null && AssemblyPKToken != null)
			{
				if (SystemPKToken.Length == AssemblyPKToken.Length)
				{
					int i=0;

					while ((i < AssemblyPKToken.Length) && (AssemblyPKToken[i] == SystemPKToken[i]))
					{
						i += 1;
					}

					if (i == AssemblyPKToken.Length)
					{
						bCustomAssembly = false;
					}
				}
			}

			return bCustomAssembly;
		}

		public static bool IsTisAssembly(AssemblyName oAssembly)
		{
			return !IsCustomAssembly(TisAssembly, oAssembly);
		}

		public static bool IsMicrosoftAssembly(AssemblyName oAssembly)
		{
            return !IsCustomAssembly(MSAssemblies, oAssembly);
		}

		public static AssemblyName TisAssembly
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName();
			}
		}

		public static AssemblyName[] MSAssemblies
		{
			get
			{
                return m_MSAssemblies;
			}
		}

		public static AssemblyName[] SystemAssemblies
		{
			get
			{
                return m_SystemAssemblies;
			}
		}


        public static ConstructorInfo GetDefaultConstructor(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
                return null;

            try
            {
                ConstructorInfo constructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    CallingConventions.HasThis,
                    Type.EmptyTypes,
                    null);

                return constructor;
            }
            catch
            {
                // No default constructor
                return null;
            }
        }

        unsafe public static bool IsTisBinary(string file)
        {
            int handle = 0;
            int size = GetFileVersionInfoSize(file, out handle);
            if (size == 0)
            {
                return false;
            }

            byte[] buffer = new byte[size];
            if (!GetFileVersionInfo(file, handle, size, buffer))
            {
                return false;
            }

            short* subBlock = null;
            uint length = 0;
            if (!VerQueryValue(buffer, @"\VarFileInfo\Translation", out subBlock, out length))
            {
                return false;
            }

            string legalCopyright;
            string versionInfoFullName = GetVersionInfoFullName("LegalCopyright", subBlock);
            if (!VerQueryValue(buffer, versionInfoFullName, out legalCopyright, out length))
            {
                return false;
            }

            if (StringUtil.IsStringInitialized(legalCopyright) && legalCopyright.Contains("TopImageSystems"))
            {
                return true;
            }

            string companyName;
            versionInfoFullName = GetVersionInfoFullName("CompanyName", subBlock);
            if (!VerQueryValue(buffer, versionInfoFullName, out companyName, out length))
            {
                return false;
            }

            if (StringUtil.IsStringInitialized(companyName) &&
                (StringUtil.CompareIgnoreCase(companyName, "Top Image Systems Ltd") ||
                StringUtil.CompareIgnoreCase(companyName, "TiS")))
            {
                return true;
            }

            return false;
        }

        unsafe private static string GetVersionInfoFullName(string tokenName, short* subBlock)
        {
            StringBuilder versionInfoFullName = new StringBuilder();

            versionInfoFullName.AppendFormat(@"\StringFileInfo\{0}{1}\{2}", 
                subBlock[0].ToString("X4"),
                subBlock[1].ToString("X4"),
                tokenName);

            return versionInfoFullName.ToString();
        }

        #region PInvoke

        [DllImport("version.dll")]
        internal static extern bool GetFileVersionInfo(string sFileName, int handle, int size, byte[] infoBuffer);
        [DllImport("version.dll")]
        internal static extern int GetFileVersionInfoSize(string sFileName, out int handle);
        [DllImport("version.dll")]
        unsafe internal static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out string pValue, out uint len);
        [DllImport("version.dll")]
       unsafe  internal static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out short* pValue, out uint len);

        #endregion

        #region ILReflector

        public static object Invoke(MethodInfo method, object target, object[] paramters)
        {
            InvokeMethodDelegate invokeMethod;

            lock (m_locker)
            {
                if (!m_methodsCache.TryGetValue(method, out invokeMethod))
                {
                    invokeMethod = ILReflector.CreateMethodInvoker(method);

                    m_methodsCache.Add(method, invokeMethod);
                }
            }

            return invokeMethod(target, paramters);
        }

        public static object CreateInstance(Type targetType)
        {
            object targetInstance = null;

            ILReflector ILReflector = GetILReflector(targetType);

            if (ILReflector != null)
            {
                targetInstance = ILReflector.CreateInstance();
            }

            return targetInstance;
        }

        public static void SetFieldValues(object target, List<object> propValues)
        {
            ILReflector ILReflector = GetILReflector(target.GetType());

            if (ILReflector != null)
            {
                ILReflector.SetFieldValues(target, propValues.ToArray());
            }
        }

        public static List<object> GetFieldValues(object target)
        {
            List<object> propValues = new List<object>();

            ILReflector ILReflector = GetILReflector(target.GetType());

            if (ILReflector != null)
            {
                propValues.AddRange(ILReflector.GetFieldValues(target));
            }

            return propValues;
        }

        public static void SetFieldValue(object target, FieldInfo fieldInfo, object fieldValue)
        {
            ILReflector ILReflector = GetILReflector(target.GetType());

            ILReflector.SetFieldValue(target, fieldInfo, fieldValue);
        }

        public static object GetFieldValue(object target, FieldInfo fieldInfo)
        {
            ILReflector ILReflector = GetILReflector(target.GetType());

            return ILReflector.GetFieldValue(target, fieldInfo);
        }

        public static object GetPropertyValue(object target, PropertyInfo propertyInfo)
        {
            ILReflector ILReflector = GetILReflector(target.GetType());

            return ILReflector.GetPropertyValue(target, propertyInfo);
        }

        public static object SetPropertyValue(object target, PropertyInfo propertyInfo, object propertyValue)
        {
            ILReflector ILReflector = GetILReflector(target.GetType());

            return ILReflector.SetPropertyValue(target, propertyInfo, propertyValue);
        }

        public static ILReflector GetILReflector(Type reflectedType)
        {
            return GetILReflector(reflectedType, BindingFlags.Default, false);
        }

        public static ILReflector GetILReflector(Type reflectedType, BindingFlags bindingFlags, bool serializedFieldsOnly)
        {
            lock (m_locker)
            {
                ILReflector ILReflector = null;

                if (reflectedType != null && !m_allILReflectors.TryGetValue(reflectedType, out ILReflector))
                {
                    ILReflector = CreateILReflector(reflectedType, bindingFlags, serializedFieldsOnly);

                    m_allILReflectors.Add(reflectedType, ILReflector);
                }


                return ILReflector;
            }
        }

        public static ILReflector CreateILReflector(Type reflectedType, BindingFlags bindingFlags, bool serializedFieldsOnly)
        {
            FieldInfo[] fields = GetTypeFieldsForILReflector(reflectedType, bindingFlags, serializedFieldsOnly);

            PropertyInfo[] properties = GetTypePropertiesForILReflector(reflectedType, bindingFlags);

            try
            {
                return new ILReflector(reflectedType, fields, properties);
            }
            catch
            {
                return null;
            }
        }

        internal static FieldInfo[] GetTypeFieldsForILReflector(
            Type reflectedType, 
            BindingFlags bindingFlags, 
            bool serializedFieldsOnly)
        {
            bool obtainAllFields = bindingFlags == BindingFlags.Default;

            ArrayBuilder oAllFields = new ArrayBuilder();

            if (obtainAllFields)
            {
                bindingFlags =
                    BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy ;
            }

            // Get all fields that belong specified type
            // Note: private fields of base types not returned
            oAllFields.AddRange(
                reflectedType.GetFields(bindingFlags),
                new ArrayElementFilter(NonLiteralFieldsFilter));


            if (obtainAllFields)
            {
                //
                //	Now iterate the base classes and add all private fields
                //

                Type oType = reflectedType;
                Type oBaseType = null;

                while ((oBaseType = oType.BaseType) != null)
                {
                    // Get non-public fields of type
                    FieldInfo[] Fields = oBaseType.GetFields(
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                    //// Add only private fields
                    oAllFields.AddRange(
                        Fields,
                        new ArrayElementFilter(PrivateFieldsFilter));

                    oType = oBaseType;
                }
            }

            ArrayElementFilter filter = null;

            if (serializedFieldsOnly)
            {
                filter = new ArrayElementFilter(SerializedFieldsFilter);
            }

            // Filter & store fields to be serialized
            return (FieldInfo[])ArrayBuilder.CreateArray(
                oAllFields.Elements,
                typeof(FieldInfo),
                filter);
        }


        internal static PropertyInfo[] GetTypePropertiesForILReflector(Type reflectedType, BindingFlags bindingFlags)
        {
            bool obtainAllFields = bindingFlags == BindingFlags.Default;

            ArrayBuilder oAllProperties = new ArrayBuilder();

            if (obtainAllFields)
            {
                bindingFlags =
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            }

            oAllProperties.AddRange(reflectedType.GetProperties(bindingFlags));

            if (obtainAllFields)
            {
                Type oType = reflectedType;
                Type oBaseType = null;

                while ((oBaseType = oType.BaseType) != null)
                {
                    // Get non-public fields of type
                    PropertyInfo[] Properties = 
                        oBaseType.GetProperties(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.FlattenHierarchy);

                    //// Add only private fields
                    oAllProperties.AddRange(
                        Properties,
                        null);

                    oType = oBaseType;
                }
            }

            // Filter & store fields to be serialized
            return (PropertyInfo[])ArrayBuilder.CreateArray(
                oAllProperties.Elements,
                typeof(PropertyInfo),
                null);
        }

        internal static bool SerializedFieldsFilter(object oObjFieldInfo)
        {
            FieldInfo oFieldInfo = (FieldInfo)oObjFieldInfo;

            // Events not serialized
            if (IsEvent(oFieldInfo))
            {
                return false;
            }

            // NonSerialized handling
            if (oFieldInfo.IsNotSerialized)
            {
                return false;
            }

            return true;
        }

        internal static bool PrivateFieldsFilter(object oObjFieldInfo)
        {
            FieldInfo oFieldInfo = (FieldInfo)oObjFieldInfo;

            return oFieldInfo.IsPrivate && !oFieldInfo.IsLiteral ;
        }

        internal static bool NonLiteralFieldsFilter(object oObjFieldInfo)
        {
            FieldInfo oFieldInfo = (FieldInfo)oObjFieldInfo;

            return !oFieldInfo.IsLiteral;
        }

        internal static bool IsEvent(FieldInfo oFieldInfo)
        {
            EventInfo oEvent = oFieldInfo.DeclaringType.GetEvent(
                oFieldInfo.Name,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance);

            if (oEvent == null)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
