using System;
using System.IO;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Customizations.MethodInvokers.Managed;
using TiS.Core.TisCommon.Customizations.MethodBrowsers.Managed;
using System.Collections.Generic;

namespace TiS.Core.TisCommon.Customizations.MethodBrowsers
{
    [Guid("AEFCA61B-C5D4-46e3-B83F-51FC6AFB16E8")]
    [ClassInterface (ClassInterfaceType.None)]
	public class TisMethodsExplorer : MarshalByRefObject, ITisMethodsExplorer, IDisposable
	{
		private string m_sFileName;
		private static TisMethodsExplorerFactory m_oExplorerFactory;
		private ITisMethodsExplorer m_oTypedExplorer = null;

        public TisMethodsExplorer() : this(new CustomAssemblyResolver(String.Empty))
        {
        }

        public TisMethodsExplorer(ICustomAssemblyResolver oAssemblyResolver)
		{
            if (m_oExplorerFactory == null)
            {
                m_oExplorerFactory = new TisMethodsExplorerFactory(oAssemblyResolver);
            }
		}

        public TisMethodsExplorer(EXPLORER_TYPE oExplorerType, ICustomAssemblyResolver oAssemblyResolver)
            : this(oAssemblyResolver)
		{
            m_oTypedExplorer = m_oExplorerFactory.GetExplorer(oExplorerType, oAssemblyResolver.CustomizationDir);
		}

        public TisMethodsExplorer(string sFileName)
		{
			m_sFileName = sFileName;
			m_oTypedExplorer = m_oExplorerFactory.GetExplorer (sFileName);
		}

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_oTypedExplorer != null)
            {
                if (!System.Runtime.Remoting.RemotingServices.IsTransparentProxy(m_oTypedExplorer))
                {
                    m_oTypedExplorer.Dispose();
                }

                m_oTypedExplorer = null;
            }

            if (m_oExplorerFactory != null)
            {
                m_oExplorerFactory.Dispose();

                m_oExplorerFactory = null;
            }
        }

        #endregion

		#region ITisMethodsExplorer Members

		public ITisMethodsExplorer GetTypedExplorer (EXPLORER_TYPE oExplorerType)
		{
			return m_oExplorerFactory.GetExplorer (oExplorerType); 
		}

        public ITisMethodsExplorer GetTypedExplorer(string sFileName)
        {
            return m_oExplorerFactory.GetExplorer(sFileName);
        }

        public virtual ITisExplorerQuery QueryMethods(string sFileName)
		{
			EnsureTypedExplorer (sFileName);

			return m_oTypedExplorer.QueryMethods (sFileName);
		}

		public virtual ITisExplorerQuery QueryMethods ()
		{
			return QueryMethods (m_sFileName);
		}

		public virtual ITisExplorerQuery QueryMethods (string sFileName, ITisQueryFilter oQueryFilter)
		{
			EnsureTypedExplorer (sFileName);

			return m_oTypedExplorer.QueryMethods (sFileName, oQueryFilter);
		}

		public virtual ITisExplorerQuery QueryMethods (ITisQueryFilter oQueryFilter)
		{
			return QueryMethods (m_sFileName, oQueryFilter);
		}

        public ITisExplorerSupportsQueryFilter SupportsQueryFilter(string sFileName)
        {
			EnsureTypedExplorer (sFileName);

            return m_oTypedExplorer as ITisExplorerSupportsQueryFilter;
        }

		#endregion

		private void EnsureTypedExplorer (string sFileName)
		{
			if (m_oTypedExplorer == null)
			{
				m_oTypedExplorer = m_oExplorerFactory.GetExplorer (sFileName);
			}
		}
    }

	[Guid("B51ADAE9-3971-4db9-8BB4-275D7A14CA29")]
	[ClassInterface (ClassInterfaceType.None)]
    [Serializable]
	public class TisExplorerQuery : MarshalByRefObject, ITisExplorerQuery
	{
		private string m_sFileName;
		private string[] m_MethodsNames;
        protected List<string> m_referencedAssemblies;

		public TisExplorerQuery (
			string sFileName,
			string[] MethodsNames)
		{
			m_sFileName    = sFileName;
			m_MethodsNames = MethodsNames;
		}

        public override object InitializeLifetimeService()
        {
            return null;
        }

		#region ITisExplorerQuery Members

		public virtual string FileName
		{
			get
			{
				return m_sFileName;
			}
		}

		public virtual string[] MethodsNames
		{
			get
			{
				return m_MethodsNames;
			}
		}

		public virtual int MethodsCount
		{
			get
			{
				return MethodsNames.Length;
			}
		}

		public virtual string MethodByIndex(int nIndex)
		{
			ValidateCollectionIndex (nIndex, MethodsCount);

			return MethodsNames[nIndex];
		}

        public string[] ReferencedAssemblies
        {
            get
            {
                return m_referencedAssemblies.ToArray(); ;
            }
        }

		#endregion

		internal void ValidateCollectionIndex (int nIndex, int nCount)
		{
			if (nIndex < 0 || nIndex >= nCount)
			{
				throw new TisException ("Index {0} is out of range [0 - {1}]", nIndex, nCount);
			}
		}
	}

	[ClassInterface (ClassInterfaceType.None)]
	internal class TisMethodsExplorerFactory
	{
        private static AppDomain m_oAssemblyExplorerDomain;
        private static IAssemblyExplorer m_managedExplorer;

        public TisMethodsExplorerFactory(ICustomAssemblyResolver oAssemblyResolver)
        {
            if (m_oAssemblyExplorerDomain == null)
            {
                AppDomainSetup domainSetup = new AppDomainSetup();

                domainSetup.ApplicationBase = oAssemblyResolver.BaseAssemblyLocation;

                m_oAssemblyExplorerDomain =
                    AppDomain.CreateDomain("AssemblyExplorerDomain", typeof(AssemblyExplorer).Assembly.Evidence, domainSetup);
            }
        }

        public void Dispose()
        {
            if (m_oAssemblyExplorerDomain != null)
            {
                AppDomain.Unload(m_oAssemblyExplorerDomain);

                m_oAssemblyExplorerDomain = null;

                m_managedExplorer = null;
            }
        }

        public ITisMethodsExplorer GetExplorer(string sFileName)
        {
            string customizationDir = Path.GetDirectoryName(sFileName);

            if (!StringUtil.IsStringInitialized(customizationDir))
            {
                customizationDir = m_oAssemblyExplorerDomain.BaseDirectory;
            }

            switch (PEImageHelper.GetPEInvokeType(sFileName))
            {
                case PEImageHelper.PEInvokeType.DNAssembly:
                    return CreateAssemblyExplorer(customizationDir);

                default:
                    throw new TisException("Not supported file format {0}", sFileName);
            }
        }

        public ITisMethodsExplorer GetExplorer(EXPLORER_TYPE oExplorerType, string customizationDir = null)
        {
            if (!StringUtil.IsStringInitialized(customizationDir))
            {
                customizationDir = m_oAssemblyExplorerDomain.BaseDirectory;
            }

            switch (oExplorerType)
            {
                case EXPLORER_TYPE.DOTNET:
                    return CreateAssemblyExplorer(customizationDir);

                default:
                    throw new TisException("Not supported explorer type");

            }
        }

        private IAssemblyExplorer CreateAssemblyExplorer(string customizationDir)
        {
            if (m_managedExplorer == null)
            {
                m_managedExplorer =
                    (IAssemblyExplorer)m_oAssemblyExplorerDomain.CreateInstanceAndUnwrap(
                                typeof(AssemblyExplorer).Assembly.FullName,
                                typeof(AssemblyExplorer).FullName);
            }

            m_managedExplorer.CustomizationDir = customizationDir;

            return m_managedExplorer;
        }
    }

}
