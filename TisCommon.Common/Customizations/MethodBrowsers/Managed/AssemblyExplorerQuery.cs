using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using TiS.Core.TisCommon.Reflection;
using System.IO;
using TiS.Core.TisCommon.Customizations.MethodInvokers.Managed;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon.Customizations.MethodBrowsers.Managed
{
    [Serializable]
    internal class AssemblyExplorerQuery : TisExplorerQuery, IAssemblyExplorerQuery
    {
        private string m_sAssemblyName;
        private IAssemblyTypeInfo[] m_AssemblyTypes;
        private CustomAssemblyResolver m_customAssemblyResolver;

        public AssemblyExplorerQuery(
            Assembly oAssembly,
            IAssemblyQueryFilter oQueryFilter,
            string customizationDir)
            : base(oAssembly.GetName().Name, new string[] { })
        {
            m_sAssemblyName = oAssembly.GetName().Name;

            m_customAssemblyResolver = new CustomAssemblyResolver(new BasicConfiguration().eFlowBinPath);

            m_customAssemblyResolver.CustomizationDir = customizationDir;

            m_AssemblyTypes = QueryAssemblyTypes(oAssembly, oQueryFilter);

            m_referencedAssemblies = QueryReferencedAssemblies(oAssembly);
        }

        #region IAssemblyExplorerQuery Members

        public string AssemblyName
        {
            get
            {
                return m_sAssemblyName;
            }
        }

        public IAssemblyTypeInfo[] AssemblyTypes
        {
            get
            {
                return m_AssemblyTypes;
            }
        }

        public int AssemblyTypesCount
        {
            get
            {
                return m_AssemblyTypes.Length;
            }
        }

        public IAssemblyTypeInfo AssemblyType(int Index)
        {
            return m_AssemblyTypes[Index];
        }

        #endregion

        #region ITisExplorerQuery Members

        public override string[] MethodsNames
        {
            get
            {
                ArrayList oMethodsNames = new ArrayList();

                foreach (IAssemblyTypeInfo oTypeInfo in m_AssemblyTypes)
                {
                    foreach (string sMethodName in oTypeInfo.TypeMethods)
                    {
                        oMethodsNames.Add(oTypeInfo.TypeName + "." + sMethodName);
                    }
                }

                return ((string[])oMethodsNames.ToArray(typeof(string)));
            }
        }

        #endregion

        private IAssemblyTypeInfo[] QueryAssemblyTypes(
            Assembly oAssembly,
            IAssemblyQueryFilter oQueryFilter)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(m_customAssemblyResolver.AssemblyResolveHandler);

            try
            {
                Type[] TypesToExplore = FilterOutTypes(oAssembly, oQueryFilter);

                ArrayList oAssemblyTypesInfo = new ArrayList();

                foreach (Type oAssemblyType in TypesToExplore)
                {
                    IAssemblyTypeInfo oAssemblyTypeInfo =
                        QueryTypeMethods(oAssemblyType, oQueryFilter);

                    if (oAssemblyTypeInfo != null)
                    {
                        oAssemblyTypesInfo.Add(oAssemblyTypeInfo);
                    }
                }

                return (IAssemblyTypeInfo[])oAssemblyTypesInfo.ToArray(typeof(IAssemblyTypeInfo));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(m_customAssemblyResolver.AssemblyResolveHandler);
            }
        }

        private IAssemblyTypeInfo QueryTypeMethods(
            Type oAssemblyType,
            IAssemblyQueryFilter oQueryFilter)
        {
            IAssemblyTypeInfo oAssemblyTypeInfo =
                    new AssemblyTypeInfo(oAssemblyType, oQueryFilter);

            if (oAssemblyTypeInfo.TypeMethodsCount > 0)
            {
                return oAssemblyTypeInfo;
            }
            else
            {
                return null;
            }
        }

        private Type[] FilterOutTypes(
            Assembly oAssembly,
            IAssemblyQueryFilter oQueryFilter)
        {
            ArrayList oAssemblyTypes = new ArrayList();

            Type[] AssemblyPublicTypes = oAssembly.GetTypes();

            Type oCheckedType = null;

            bool bFilterByType = oQueryFilter != null &&
                oQueryFilter.TypeName != null &&
                oQueryFilter.TypeName != String.Empty;

            if (bFilterByType)
            {
                oCheckedType = AssemblyTypeInfo.GetTypeByName(oQueryFilter.TypeName, oAssembly);
            }

            foreach (Type oAssemblyType in AssemblyPublicTypes)
            {
                bool bShouldBeConsidered =
                    oAssemblyType.IsClass &&
                    (oAssemblyType.IsPublic || oAssemblyType.IsNestedPublic);

                if (bShouldBeConsidered && bFilterByType)
                {
                    bShouldBeConsidered = oCheckedType != null &&
                                          oCheckedType.Equals(oAssemblyType);
                }

                if (bShouldBeConsidered)
                {
                    oAssemblyTypes.Add(oAssemblyType);
                }
            }

            return (Type[])oAssemblyTypes.ToArray(typeof(Type));
        }

        public List<string> QueryReferencedAssemblies(Assembly oAssembly)
        {
            List<string> referencedAssemblies = new List<string>();

            try
            {
                ArrayBuilder helperArray = new ArrayBuilder(typeof(string));

                ReflectionUtil.GetAssemblyLocation(
                    oAssembly,
                    helperArray,
                    ReflectionUtil.SystemAssemblies,
                    true,
                    true,
                    false,
                    m_customAssemblyResolver);

                foreach (string referencedAssembly in helperArray.Elements)
                {
                    referencedAssemblies.Add(Path.GetFileName(referencedAssembly));
                }
            }
            catch (Exception exc)
            {
                Log.WriteException(exc);
            }

            return referencedAssemblies;
        }

    }
}
