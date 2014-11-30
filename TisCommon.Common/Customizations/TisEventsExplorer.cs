using System;
using System.Collections;
using System.IO;
using TiS.Core.TisCommon.Customizations.MethodInvokers.Managed;
using TiS.Core.TisCommon.Customizations.MethodBrowsers;

namespace TiS.Core.TisCommon.Customizations
{
	internal class TisEventsExplorer : ITisEventsExplorer
	{
		private string m_sFileName;
		private ITisMethodsExplorer m_oMethodsExplorer;
        private ICustomAssemblyResolver m_oAssemblyResolver;

        public TisEventsExplorer(ICustomAssemblyResolver oAssemblyResolver)
		{
            m_oAssemblyResolver = oAssemblyResolver;

            m_oMethodsExplorer = new TisMethodsExplorer(oAssemblyResolver);
		}

        public TisEventsExplorer(EXPLORER_TYPE oExplorerType, ICustomAssemblyResolver oAssemblyResolver)
		{
            m_oAssemblyResolver = oAssemblyResolver;

            m_oMethodsExplorer = new TisMethodsExplorer(oExplorerType, oAssemblyResolver);
		}


        #region IDisposable Members

        public void Dispose()
        {
            (m_oMethodsExplorer as IDisposable).Dispose();
        }

        #endregion

        #region ITisEventsExplorer Members

        public ITisInvokeParams[] GetMatchingMethodsBySignature(string sFileName, ITisMethodSignature oMethodSignature)
		{
            m_oAssemblyResolver.ValidateFile(ref sFileName);

			m_sFileName = sFileName;

            ITisQueryFilter oQueryFilter = GetQueryFilter(sFileName, oMethodSignature);

			ITisExplorerQuery oQuery = m_oMethodsExplorer.QueryMethods (sFileName, oQueryFilter);

            return GetInvokeParams(oQuery);
		}

        #endregion

        public string[] GetReferencedAssemblies(string sFileName)
        {
            //m_oAssemblyResolver.ValidateFile(ref sFileName);

            m_sFileName = sFileName;

            ITisExplorerQuery oQuery = m_oMethodsExplorer.QueryMethods(sFileName);

            return oQuery.ReferencedAssemblies == null ? new string[0] : oQuery.ReferencedAssemblies;
        }

        private ITisInvokeParams[] GetInvokeParams(ITisExplorerQuery oQuery)
		{
			ArrayList oInvokeParamsList = new ArrayList (); 

			if (oQuery is IAssemblyExplorerQuery)
			{
				IAssemblyTypeInfo oTypeInfo;

				for (int i = 0; i < (oQuery as IAssemblyExplorerQuery).AssemblyTypesCount; i++)
				{
					oTypeInfo = (oQuery as IAssemblyExplorerQuery).AssemblyType (i);

					for (int j = 0; j < oTypeInfo.TypeMethodsCount; j++)
					{
						oInvokeParamsList.Add (new TisDNInvokeParams (
							(oQuery as IAssemblyExplorerQuery).AssemblyName,
							oTypeInfo.TypeName,
							oTypeInfo.TypeMethod (j)));
					}
				}
			}
			else
			{
				if (oQuery is IVBAExplorerQuery)
				{
					IVBAModuleInfo oModuleInfo;

					for (int i = 0; i < (oQuery as IVBAExplorerQuery).ModulesCount; i++)
					{
						oModuleInfo = (oQuery as IVBAExplorerQuery).Module (i);

						for (int j = 0; j < oModuleInfo.FunctionsCount; j++)
						{
							oInvokeParamsList.Add (new TisVBAInvokeParams (
								(oQuery as IVBAExplorerQuery).FileName,
								oModuleInfo.ModuleName,
								oModuleInfo.FunctionName (j)));
						}

					}
				}
				else
				{
					for (int i = 0; i < oQuery.MethodsCount; i++)
					{
						oInvokeParamsList.Add (new TisWin32DLLInvokeParams (
							oQuery.FileName,
							oQuery.MethodByIndex (i)));
					}
				}
			}

			return (ITisInvokeParams[]) oInvokeParamsList.ToArray (typeof (ITisInvokeParams));								
		}

		private ITisQueryFilter GetQueryFilter (string sFileName, ITisMethodSignature oMethodSignature)
		{
            ITisExplorerSupportsQueryFilter oExplorerWithQueryFilter =
                m_oMethodsExplorer.SupportsQueryFilter(sFileName);

            if (oExplorerWithQueryFilter != null)
			{
				ArrayList oArrayList = new ArrayList (); 

				foreach (ITisMethodParam oMethodParam in oMethodSignature.Params)
				{
					oArrayList.Add (oMethodParam.ParamType.AssemblyQualifiedName);
				}

                return oExplorerWithQueryFilter.GetQueryFilter(
                    String.Empty,
					oMethodSignature.Params.Length,
					(string[]) oArrayList.ToArray (typeof (string)),
					oMethodSignature.ReturnInfo.ReturnType.Name);
			}
			else
			{
				return null;
			}
		}
    }
}
