using System;
using System.Runtime.InteropServices;


namespace TiS.Core.TisCommon.Customizations.MethodBrowsers
{
	public enum EXPLORER_TYPE {DOTNET, WIN32DLL};

	[Guid ("28C69484-2E71-4617-B090-30C27440056C")]
	public interface ITisMethodsExplorer : IDisposable
	{
		ITisExplorerQuery QueryMethods (string sFileName);
		ITisExplorerQuery QueryMethods (string sFileName, ITisQueryFilter oQueryFilter);
		ITisExplorerQuery QueryMethods (ITisQueryFilter oQueryFilter);
		ITisExplorerQuery QueryMethods ();
		ITisMethodsExplorer GetTypedExplorer (EXPLORER_TYPE oExplorerType);
        ITisMethodsExplorer GetTypedExplorer (string sFileName);
        ITisExplorerSupportsQueryFilter SupportsQueryFilter(string sFileName);
    }

	[Guid ("8AEBC662-114F-4819-9FA0-F94B654EDDBC")]
	public interface ITisExplorerQuery
	{
		string FileName	{get;}
		string[] MethodsNames {get;}
		int MethodsCount {get;}
		string MethodByIndex (int Index);
        string[] ReferencedAssemblies { get; }
    }

	[Guid ("93A73BB1-11C3-4587-BBD5-83A446DAA427")]
	public interface ITisExplorerSupportsQueryFilter
	{
		ITisQueryFilter GetQueryFilter (string sClassName,
			int nMethodParamsCount,
			[MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_VARIANT)]
			string[] MethodParamsTypeNames,
			string sMethodReturnTypeName);
	}

	[Guid ("8A562E44-B014-4b70-93BC-BFF29B943AFD")]
	public interface ITisQueryFilter
	{
	}

	#region VBA
	[Guid ("DE3FBB8F-0EE9-4041-8562-01B98D7DB6FB")]
	public interface IVBAExplorer : ITisMethodsExplorer
	{
//		IVBAExplorerQuery QueryMethods (string sVBAProjectName);
	}

	[Guid ("72E8B923-312B-401c-BC8C-C4A3781B35C9")]
	public interface IVBAExplorerQuery : ITisExplorerQuery
	{
		IVBAModuleInfo[] Modules {get;}
		int ModulesCount {get;}
		IVBAModuleInfo Module (int nIndex);
	}

	[Guid ("107288DD-2EC6-4b54-8EA5-97C08CC248A7")]
	public interface IVBAModuleInfo
	{
		string ModuleName {get;}
		string[] FunctionsNames {get;}
		int FunctionsCount {get;}
		string FunctionName (int nIndex);
	}
	#endregion

	#region Win32DLL 
	#endregion

	#region Mamaged Assembly
	[Guid ("18070947-D7B5-466d-ADF1-C2C787CA4F61")]
	public interface IAssemblyExplorer : ITisMethodsExplorer
	{
		IAssemblyExplorerQuery QueryAllMethods (string sAssemblyName);

		IAssemblyExplorerQuery QueryMethods (
			string sAssemblyName,
			string TypeNameFilter,
			int MethodParamsCountFilter,
			[MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_VARIANT)]
			string[] MethodTypeOfParamsFilter,
			string MethodReturnTypeFilter);

		IAssemblyExplorerQuery QueryMethodsEx (
			string sAssemblyName,
			IAssemblyQueryFilter QueryFilter);

        string CustomizationDir { get; set; }
	}

	[Guid ("EE501231-77B9-4fb8-9BF4-DC018EB5EEC4")]
	public interface IAssemblyExplorerQuery : ITisExplorerQuery
	{
		string AssemblyName	{get;}
		IAssemblyTypeInfo[] AssemblyTypes {get;}
		int AssemblyTypesCount {get;}
		IAssemblyTypeInfo AssemblyType (int Index);
	}

	[Guid ("4A2EDEC6-1033-41bd-A8A8-5157B9FFF3A6")]
	public interface IAssemblyTypeInfo
	{
		string TypeName {get;}
		string[] TypeMethods {get;}
		int TypeMethodsCount {get;}
		string TypeMethod (int Index);
	}

	[Guid ("466CA311-5431-48b4-BF5B-1D57AA15876E")]
	public interface IAssemblyQueryFilter : ITisQueryFilter
	{
		string TypeName {get;}
		int MethodParamsCount {get;}
		string[] MethodTypeOfParams {get;}
		string MethodReturnType {get;}
	}
	#endregion
}
