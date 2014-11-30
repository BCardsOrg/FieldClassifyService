using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace TiS.Core.TisCommon.Customizations.MethodBrowsers.Managed
{
	[Guid("799B8D10-A83D-3D56-9D91-08F14A1D6E42")]
	[ClassInterface (ClassInterfaceType.None)]
	public class AssemblyExplorer : TisMethodsExplorer, IAssemblyExplorer, ITisExplorerSupportsQueryFilter
	{
		private const IAssemblyQueryFilter EMPTY_QUERY_FILTER = null;

		#region IAssemblyExplorer Members

        public string CustomizationDir { get; set; }

        public IAssemblyExplorerQuery QueryAllMethods(string sAssemblyName)
		{
			return QueryMethodsEx (
				sAssemblyName, 
				EMPTY_QUERY_FILTER);
		}

		public IAssemblyExplorerQuery QueryMethods (
			string sAssemblyName,
			string TypeNameFilter,
			int MethodParamsCountFilter,
			[MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_VARIANT)]
			string[] MethodTypeOfParamsFilter,
			string MethodReturnTypeFilter)
		{
			IAssemblyQueryFilter oQueryFilter = (IAssemblyQueryFilter) GetQueryFilter (
				TypeNameFilter,
				MethodParamsCountFilter, 
				MethodTypeOfParamsFilter,
				MethodReturnTypeFilter);

			return QueryMethodsEx (
				sAssemblyName, 
				oQueryFilter);
		}

		public IAssemblyExplorerQuery QueryMethodsEx (
			string sAssemblyName,
			IAssemblyQueryFilter QueryFilter)
		{
			Assembly oAssembly;

            try
			{
                oAssembly = Assembly.LoadFile(sAssemblyName);
			}
			catch (Exception oExc) 
			{
				Log.WriteException (oExc);

				return null;
			}

			AssemblyQueryFilter oQueryFilter = null;

			if (QueryFilter != EMPTY_QUERY_FILTER)
			{
				oQueryFilter = new AssemblyQueryFilter (
					QueryFilter.TypeName,
					QueryFilter.MethodParamsCount, 
					QueryFilter.MethodTypeOfParams, 
					QueryFilter.MethodReturnType);
			}

			return new AssemblyExplorerQuery (
				oAssembly, 
				oQueryFilter,
                CustomizationDir);
		}

		#endregion

		#region ITisMethodsExplorer Members

		public override ITisExplorerQuery QueryMethods(string sFileName)
		{
			return QueryAllMethods (sFileName);
		}

		public override ITisExplorerQuery QueryMethods (string sFileName, ITisQueryFilter oQueryFilter)
		{
			if (oQueryFilter is IAssemblyQueryFilter)
			{
				return QueryMethodsEx (sFileName, oQueryFilter as IAssemblyQueryFilter);
			}
			else
			{
				return QueryMethods (sFileName);
			}
		}

		#endregion

		#region ITisExplorerSupportsQueryFilter Members

		public ITisQueryFilter GetQueryFilter(
			string sClassName,
			int nMethodParamsCount,
			[MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_VARIANT)]
			string[] MethodParamsTypeNames,
			string sMethodReturnTypeName)
		{
			return new AssemblyQueryFilter (
                sClassName,
				nMethodParamsCount, 
				MethodParamsTypeNames,
				sMethodReturnTypeName);
		}

		#endregion
	}


}
