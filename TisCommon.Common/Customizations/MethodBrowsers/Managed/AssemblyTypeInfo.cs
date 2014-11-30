using System;
using System.Collections;
using System.Reflection;

namespace TiS.Core.TisCommon.Customizations.MethodBrowsers.Managed
{
    [Serializable]
	internal class AssemblyTypeInfo : MarshalByRefObject, IAssemblyTypeInfo
	{
        private Type m_TypeToExplore;
		private Assembly m_oAssembly;
		private string m_sShortTypeName;
		private string[] m_TypeMethods;

        public readonly static string[] TIS_STANDARD_ASSEMBLIES = new string[] {
            "TiS.Core.TisCommon.Common",
			"TiS.Core.TisCommon.Client",
			"TiS.Core.Domain.Common",
			"TiS.Core.Domain.Client",
			"TiS.Core.Application.Common",
			"TiS.Core.Application.Client"};

        public readonly static string[] TIS_STANDARD_NAMESPACES = new string[] {
			"MSXML"};

        public AssemblyTypeInfo(
			Type oTypeToExplore, 
			IAssemblyQueryFilter oQueryFilter)
		{
			m_TypeToExplore = oTypeToExplore;

			m_oAssembly = m_TypeToExplore.Assembly;

			m_sShortTypeName = GetShortTypeName ();

			m_TypeMethods = ExploreTypeMethods (oQueryFilter); 
		}

		#region IAssemblyTypeInfo Members

		public string TypeName
		{
			get
			{
                 
				return m_sShortTypeName;
			}
		}

		public string[] TypeMethods
		{
			get
			{
				return m_TypeMethods;
			}
		}

		public int TypeMethodsCount
		{
			get
			{
				return m_TypeMethods.Length; 
			}
		}

		public string TypeMethod (int Index)
		{
			return m_TypeMethods [Index];
		}

		#endregion

		internal static Type GetTypeByName (string sTypeName, params object[] AssemblyProps)
		{
			const string REF_MODIFIER  = "ref "; 
			const string SYSTEM_PREFIX = "System"; 

			int nIdx;
			string sInternalTypeName = sTypeName;

			Type oType = null;

			if (sTypeName != String.Empty)
			{
				nIdx = sTypeName.ToLower().IndexOf (REF_MODIFIER);

				if (nIdx > -1)
				{
					sInternalTypeName = sTypeName.Substring (REF_MODIFIER.Length + nIdx) + "&";
				}

				oType = Type.GetType (sInternalTypeName);

				if (oType == null)
				{
                    oType = Type.GetType(SYSTEM_PREFIX + Type.Delimiter + sInternalTypeName);

                    if (oType == null)
                    {
                        oType = GetTypeByNameFromStandardAsseblies(sInternalTypeName);

                        if (oType == null)
                        {
                            oType = GetTypeByNameFromAsseblies(sInternalTypeName, AssemblyProps);
                        }
                    }
				}
			}

			return oType;
		}

        internal static Type GetTypeByNameFromStandardAsseblies(string sTypeName)
        {
            ArrayBuilder oAssemblies = new ArrayBuilder(typeof(object));

            oAssemblies.AddRange(TIS_STANDARD_ASSEMBLIES);

            return GetTypeByNameFromAsseblies(sTypeName, (object[])oAssemblies.GetArray());
        }

        internal static Type GetTypeByNameFromAsseblies(string sTypeName, object[] Assemblies)
        {
            string sFullTypeName = String.Empty;

            Type oType = null;

            foreach (object oAssembly in Assemblies)
            {
                if (oAssembly is Assembly)
                {
                    Assembly oAssemblyToSearch = oAssembly as Assembly;

                    sFullTypeName = oAssemblyToSearch.GetName().Name + Type.Delimiter + sTypeName;

                    oType = oAssemblyToSearch.GetType(sFullTypeName);
                }
                else
                {
                    if (oAssembly is string)
                    {
                        string sAssemblyName = (string)oAssembly;

                        foreach (string sNameSpace in TIS_STANDARD_NAMESPACES)
                        {
                            sFullTypeName = sNameSpace + Type.Delimiter + sTypeName + ", " + sAssemblyName;

                            oType = Type.GetType(sFullTypeName);

                            if (oType != null)
                            {
                                break;
                            }
                        }
                    }
                }

                if (oType != null)
                {
                    break;
                }
            }

            return oType;
        }

		private string[] ExploreTypeMethods (IAssemblyQueryFilter oQueryFilter)
		{
			MethodInfo[] TypePublicMethods = FilterOutTypeMethods (
				m_TypeToExplore.GetMethods (),
				oQueryFilter);

			ArrayList m_MethodNames = new ArrayList ();

			foreach (MethodInfo oMethod in TypePublicMethods)
			{
				if (!m_MethodNames.Contains (oMethod.Name))
				{
					m_MethodNames.Add (oMethod.Name);
				}
			}

			return (string []) m_MethodNames.ToArray (typeof(string));
		}

		private MethodInfo[] FilterOutTypeMethods (
			MethodInfo[] TypePublicMethods,
			IAssemblyQueryFilter oQueryFilter)
		{
			bool bFilterByParamsCount = oQueryFilter != null && oQueryFilter.MethodParamsCount > -1;

			Type [] TypeArray = null;

			bool bFilterByParamsType  = oQueryFilter != null &&
				                        oQueryFilter.MethodTypeOfParams != null;

			if (bFilterByParamsType)
			{
				TypeArray = GetTypesByName (oQueryFilter.MethodTypeOfParams);
			}

			Type oReturnType  = null;

			bool bFilterByReturnType  = oQueryFilter != null &&
				                        oQueryFilter.MethodReturnType != null;
 
			if (bFilterByReturnType)
			{
				oReturnType = GetTypeByName (oQueryFilter.MethodReturnType, m_oAssembly);

				bFilterByReturnType = oReturnType != null;
			}

			ArrayList oFilteredMethods = new ArrayList ();

			oFilteredMethods.AddRange (TypePublicMethods);

			if (bFilterByParamsCount || bFilterByParamsType || bFilterByReturnType)
			{
                using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
                {
                    foreach (MethodInfo oMethodInfo in TypePublicMethods)
                    {
                        bool bShouldBeFilteredOut = bFilterByParamsCount &&
                                                    oQueryFilter.MethodParamsCount != oMethodInfo.GetParameters().Length;

                        if (!bShouldBeFilteredOut && bFilterByParamsType)
                        {
                            bShouldBeFilteredOut =
                                m_TypeToExplore.GetMethod(
                                oMethodInfo.Name,
                                BindingFlags.ExactBinding |
                                BindingFlags.IgnoreCase |
                                BindingFlags.Public |
                                BindingFlags.Instance |
                                BindingFlags.Static,
                                null,
                                TypeArray,
                                new ParameterModifier[0]) == null;
                        }

                        if (!bShouldBeFilteredOut && bFilterByReturnType)
                        {
                            bShouldBeFilteredOut = oMethodInfo.ReturnType != oReturnType;
                        }

                        if (bShouldBeFilteredOut)
                        {
                            oFilteredMethods.Remove(oMethodInfo);
                        }
                    }
                }
			}

			return (MethodInfo []) oFilteredMethods.ToArray (typeof(MethodInfo));
		}

		private Type[] GetTypesByName (string[] TypeNames)
		{
			ArrayList oSystemTypes = new ArrayList ();

			foreach (string sTypeName in TypeNames)
			{
				Type oType = GetTypeByName (sTypeName, m_oAssembly);

				if (oType != null)
				{
					oSystemTypes.Add (oType);
				}
			}

			return (Type []) oSystemTypes.ToArray (typeof (Type));
		}

		private string GetShortTypeName ()
		{
			string sAssemblyName = m_oAssembly.GetName().Name;
 
			int nIdx = m_TypeToExplore.Namespace.IndexOf (sAssemblyName);

			if (!StringUtil.CompareIgnoreCase(m_TypeToExplore.Namespace, sAssemblyName) && 
				nIdx > - 1 && m_TypeToExplore.Namespace [sAssemblyName.Length] == '.')
			{
				return m_TypeToExplore.FullName.Substring (sAssemblyName.Length + 1);
			}
			else
			{
				return m_TypeToExplore.Name;
			}
		}
	}
}
