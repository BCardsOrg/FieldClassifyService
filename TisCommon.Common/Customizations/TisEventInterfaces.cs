using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using TiS.Core.TisCommon.Transactions;

namespace TiS.Core.TisCommon.Customizations
{
	public enum TisParameterAttribute {In, Out};

	public enum TisInvokeType {VBA, DLL, COM, DOTNET};

	public enum TIS_DYNAMIC_OBJECT 
	{
		TIS_DYNAMIC_COLLECTION,
		TIS_DYNAMIC_FOLDER,
		TIS_DYNAMIC_FORM,
		TIS_DYNAMIC_PAGE,
		TIS_DYNAMIC_FIELD_GROUP,
		TIS_DYNAMIC_FIELD_TABLE,
		TIS_DYNAMIC_FIELD_ARRAY,
		TIS_DYNAMIC_FIELD
	};


    [ComVisible(false)]
    public delegate void TisGetInvokerDelegate(string sTypeName, TisInvokeTypeMiscellaneous oMiscellaneous);

    [Guid("A4C5472C-6A6C-4fc3-A29F-6BD59DB993DD")]
    public interface ITisEventsManager : IDisposable
    {
        bool DebugMode { get; set; }
        bool DisableEvents { get; set; }

        ITisEventBindings EventBindings { get; }
        ITisEventBindingsLegacy EventBindingsLegacy { get; }

        ITisEventParams[] GetSupportedEvents(object oEventSource);
        IList GetSupportedEventNames(object oEventSource);
        IList GetSupportedEventNames(Type oEventSourceType);
        IList GetSupportedEventNames(string sEventSourceTypeName);
        IList GetInstalledInvokeTypeNames();
        ITisEventParams GetEvent(object oEventSource, string sEventName);
        ITisInvokeParams[] GetMatchingMethodsBySignature(string sFileName, ITisMethodSignature oMethodSignature);
        ITisInvokeParams[] GetMatchingMethodsByEvent(string sFileName, object oEventSource, string sEventName);
        object FireEvent(object oEventSource, string sEventName, ref object[] InOutParams);
        object FireEvent(object oEventSource, ITisEventParams oEventParams, ref object[] InOutParams);

        ITransactable EventsTransaction { get; }

        event EventHandler OnDisposing;
        string ApplicationName { get; }
    }

    [Guid("CE024DAF-5546-494d-97F6-AE1C16005C6B")]
    public interface ITisEventBindingsLegacy
    {
        ITisInvokeParams GetBinding(object oEventSource, string sEventName);
        ITisEventBinding AddBinding(object oEventSource, string sEventName, string sEventString);
    }

    [Guid("36FC2FF7-2933-4f2d-B7A9-43DF6A547C6C")]
    public interface ITisEventBindings
    {
        ITisEventBinding[] All { get; }
        int Count { get; }

        ITisEventBinding AddDNBinding(
            object oEventSource,
            string sEventName,
            string sModuleName,
            string sClassName,
            string sMethodName);

        ITisEventBinding AddVBABinding(
            object oEventSource,
            string sEventName,
            string sClassName,
            string sMethodName);

        ITisEventBinding AddWin32DLLBinding(
            object oEventSource,
            string sEventName,
            string sModuleName,
            string sMethodName);

        ITisEventBinding AddBinding(object oEventSource, string sEventName, ITisInvokeParams oInvokeParams);
        ITisEventBinding AddBinding(object oEventSource, string sEventName, MethodInfo oMethodInfo);
        ITisEventBinding AddBinding(object oEventSource, string sEventName, TisInvokeType enInvokeType, string sEventString);
        ITisEventBinding AddBinding(object oEventSource, string sEventName, string sInvokeTypeName, string sEventString);

        void RemoveBinding(object oEventSource, string sEventName);

        bool Contains(object oEventSource);
        bool Contains(object oEventSource, string sEventName);

        ITisInvokeParams GetBinding(object oEventSource, string sEventName);
        ITisEventBinding[] GetBindings(object oEventSource);

        void Clear();
    }

	[Guid ("B7FD492A-D48E-4aa7-891C-770A670D42EB")]
	public interface ITisEventParams
	{
		string Name {get;}
		Type EventHandlerType {get;}
		ITisMethodSignature MethodSignature {get;}
		void AddEventHandler (object oTarget, Delegate oDelegate);
        void RemoveEventHandler(object oTarget, Delegate oDelegate);
    }

	[Guid ("AF17B5C0-3F3E-41fd-9F39-31D1A05D1958")]
	public interface ITisMethodSignature
	{
		MethodParam[] Params {get;}
		string[] ParamNames {get;}
		Type[] ParamTypes {get;}
		MethodReturn ReturnInfo {get;}
	}

	[Guid ("F407B47A-33EF-4385-9BF8-D896936E47A8")]
	public interface ITisMethodReturn
	{
		Type ReturnType {get;}
		bool HasMarshal {get;}
		UnmanagedType MarshalType {get;}
	}

	[Guid ("06DC4131-8DE3-4f53-896D-2A1BD0B89B4E")]
	public interface ITisMethodParam
	{
		string ParamName {get;}
		Type ParamType {get;}
		bool IsOut {get;}
		bool HasMarshal {get;}
		UnmanagedType MarshalType {get;}
	}

	[Guid ("E00941BD-05AF-4029-8CDE-6B8BB69C1E7B")]
	public interface ITisInvokeParams
	{
		string InvokeType  {get; set;}
		string ModuleName  {get; set;}
		string ClassName   {get; set;}
		string MethodName  {get; set;}
		string EventString {get; set;}
	}

	[Guid ("DD309B16-922D-4651-9B20-489E78C4EFF0")]
	public interface ITisEventBinding
	{
		string EventName {get; set;}
		ITisInvokeParams InvokeParams {get; set;}
	}

    [ComVisible(false)]
    public delegate bool IDEProcessSyncDelegate();

    [ComVisible(false)]
    public delegate void OnDebuggerTerminateDelegate(ITisDebugger oSender);

    [ComVisible(false)]
    public interface ITisDebugger : IDisposable
    {
        string TypeName { get; }
        string Description { get; }
        string InvokeTypeName { get; }
        System.Diagnostics.Process DebuggedProcess { get;}
        bool IsInDebugMode { get; }
        bool IsDebuggedProcessRunning { get; }
        bool IsAvailable { get; }
        void Run(ITisInvokeParams oInvokeParams);
        void Detach(bool disposing);
        event OnDebuggerTerminateDelegate OnTerminate;
    }

    [Guid("4DC89943-4A10-4de2-ADCA-61BB60F90913")]
	public interface ITisInvokeType : IDisposable
	{
		string TypeName {get;}
		string[] GetBinTypes ();
		ITisEventInvoker GetEventsInvoker ();
		ITisEventsBrowser GetEventsBrowser (ITisInvokeParams oInvokeParams);
		ITisEventsExplorer GetEventsExplorer ();

		TisInvokeTypeMiscellaneous Miscellaneous {get;}
		event TisGetInvokerDelegate OnGetInvoker;
	}

	[Guid ("252560A1-BB0A-4a58-8BA8-7C6A836958DC")]
	public interface ITisEventInvoker : IDisposable
	{
		bool DebugMode {get; set;}

		object Invoke (ITisInvokeParams oInvokeParams, ITisEventParams oEventParams, ref object[] InOutParams);
	}

	[Guid ("D27D0E87-9210-4e28-B520-E3203D8DD88C")]
	public interface ITisEventsExplorer : IDisposable
	{
        ITisInvokeParams[] GetMatchingMethodsBySignature(string sFileName, ITisMethodSignature oMethodSignature);
    }

	[Guid ("905DFDF1-4437-4b92-936B-A8796251D65F")]
	public interface ITisEventsBrowser
	{
		ITisInvokeParams BrowseEvents (ITisMethodSignature oMethodSignature);
	}

	[Guid ("8CB086D2-491D-49d2-B7E9-FBB57E7D3656")]
	public interface ITisEventsProvider
	{
		object GetEventsProvider ();
	}

	[Guid ("B08C60A6-2805-4c23-BE17-975E105492FE")]
	public interface ITisSupportEvents
	{
	}


    [Guid("7F406A9C-1688-4057-980F-53CBCDBDE6FD")]
    public interface ITisSupportPredefinedEvents
    {
        ITisEventBinding[] GetPredefinedEvents();
        ITisEventBinding GetPredefinedEvent(string sEventName);
    }

    [Guid("BF8E8801-3CAA-4f4e-AE9F-63726BED466B")]
	public interface ITisEventControl
	{
		bool DebugMode {get; set;}

		#region no params

		void FireNoParamEvent (
			string sEventName, 
			string sSetupObjectName,
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		object FireNoParamEventEx (
			string sEventName, 
			string sSetupObjectName,
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		#endregion

		#region 0 input params

		void FireEvent_0I_1IO (
			string sEventName, 
			ref object oInOutParam1, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_0I_2IO (
			string sEventName, 
			ref object oInOutParam1,
			ref object oInOutParam2,
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_0I_3IO (
			string sEventName, 
			ref object oInOutParam1,
			ref object oInOutParam2,
			ref object oInOutParam3,
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		#endregion

		#region 1 input param
 
		void FireEvent_1I_0IO (
			string sEventName, 
			object oInParam1, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_1I_1IO (
			string sEventName, 
			object oInParam1, 
			ref object oInOutParam1, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_1I_2IO (
			string sEventName, 
			object oInParam1, 
			ref object oInOutParam1, 
			ref object oInOutParam2, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_1I_3IO (
			string sEventName, 
			object oInParam1, 
			ref object oInOutParam1, 
			ref object oInOutParam2, 
			ref object oInOutParam3, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		#endregion

		#region 2 input param
 
		void FireEvent_2I_0IO (
			string sEventName, 
			object oInParam1, 
			object oInParam2, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_2I_1IO (
			string sEventName, 
			object oInParam1, 
			object oInParam2, 
			ref object oInOutParam1, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_2I_2IO (
			string sEventName, 
			object oInParam1, 
			object oInParam2, 
			ref object oInOutParam1, 
			ref object oInOutParam2, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_2I_3IO (
			string sEventName, 
			object oInParam1, 
			object oInParam2, 
			ref object oInOutParam1, 
			ref object oInOutParam2, 
			ref object oInOutParam3, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		#endregion

		#region 3 input param
 
		void FireEvent_3I_0IO (
			string sEventName, 
			object oInParam1, 
			object oInParam2, 
			object oInParam3, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_3I_1IO (
			string sEventName, 
			object oInParam1, 
			object oInParam2, 
			object oInParam3, 
			ref object oInOutParam1, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_3I_2IO (
			string sEventName, 
			object oInParam1, 
			object oInParam2, 
			object oInParam3, 
			ref object oInOutParam1, 
			ref object oInOutParam2, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		void FireEvent_3I_3IO (
			string sEventName, 
			object oInParam1, 
			object oInParam2, 
			object oInParam3, 
			ref object oInOutParam1, 
			ref object oInOutParam2, 
			ref object oInOutParam3, 
			string sSetupObjectName, 
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		#endregion

		void FireEvent (
			string sEventName,
			object[] InParams,
			ref object[] InOutParams,
			string sSetupObjectName,
			TIS_DYNAMIC_OBJECT enDynamicObjectType);

		object FireEventEx (
			string sEventName,
			object[] InParams,
			ref object[] InOutParams,
			string sSetupObjectName,
			TIS_DYNAMIC_OBJECT enDynamicObjectType);
	}

	[Guid("E9BA1958-999E-4a27-8A19-A986F771F0B1")]
	public interface ITisEventFirer
	{
	}


	#region Customization project

    //[Guid("8C2068F2-CE6D-4a15-A909-19C51C1C4925")]
    //public interface ITisSolutionManager : ITisGenericEntity
    //{
    //    // Service

    //    string[] GetSolutionTypes();
    //    IList    GetSolutionTypesList();

    //    ITisSolution[] All {get;}
    //    ITisSolution GetSolution(string sSolutionType);
    //    ITisSolution GetSolutionByInvokeType(string sInvokeType);
    //    void SaveAll();
    //    void SaveSolution(string sInvokeType);
    //    ITisMethodInfo AddEvent(
    //        string sInvokeType, 
    //        string sProjectType, 
    //        string sEventString, 
    //        ITisEventParams oEventParams,
    //        ITisProjectDebugInfo oProjectDebugInfo);
    //    bool NeedsBuilding {get;}
    //    ITisStationInstance Session { get; set;}
    //    bool IsDirty { get;}
    //}

    //public delegate void OnSolutionPreOpenDelegate(ITisSolution oSolution);

    //[Guid("86311123-9BEF-444f-ACEC-710E7F417F14")]
    //public interface ITisSolution : ITisGenericEntity
    //{
    //    // Service

    //    string SolutionName {get;}
    //    string SolutionArchiveDir {get; set;}
    //    string SolutionDir {get; set;}
    //    string SolutionFile {get;}
    //    string SolutionFileExt {get;}
    //    string SolutionType {get;}
    //    string StandardReferencesDir {get; set;}
    //    string[] ProjectTypes {get;}
    //    bool  NeedsBuilding {get;}
    //    string InvokeType  {get;}

    //    void Open();
    //    void Save();
    //    void Build(out bool bCompiled, out string sCompilerErrors);
    //    void Close();

    //    event OnSolutionDelegate OnSave;
    //    event OnSolutionPreOpenDelegate OnPreOpen;

    //    ITisProjectManager[] AllProjectManagers {get;}
    //    ITisProjectManager[] GetProjectManagers();
    //    IList GetProjectManagersList();

    //    ITisProjectManager GetProjectManager(string sProjectType);
    //    ITisProjectManager GetProjectManagerByLanguage(TisSupportedLanguage enLanguage);

    //    void EnsureBinariesExist();
    //    string GetMethodPrototype (string sName, ITisMethodSignature oMethodSignature);
    //    bool IsOpened { get;}
    //}

    //[Guid("184D9EE9-C58B-43ed-88AE-02DE1EE776F7")]
    //public interface ITisProjectManager : ITisGenericEntity 
    //{
    //    // Service

    //    string SolutionType {get;}
    //    ITisProjectInfo[] Projects {get;}
    //    ITisProjectInfo AddProject(
    //        string sProjectName,
    //        Type[] typesToBeReferenced,
    //        ITisProjectDebugInfo oProjectDebugInfo);
    //    void AddProjects(ICollection Projects);
    //    ITisProjectInfo GetProject(string sProjectName);

    //    ITisMethodInfo AddEvent(
    //        string sEventString, 
    //        ITisEventParams oEventParams,
    //        ITisProjectDebugInfo oProjectDebugInfo);

    //    void Save(string sProjectName);
    //    void Build(string sProjectName, out bool bCompiled, out string sCompilerOutput);

    //    bool IsDirty(string sProjectName);
    //}

    //[Guid("E540FB3E-1032-44a3-A60C-DACD6DF73172")]
    //public interface ITisProjectInfo : ITisGenericEntity
    //{
    //    string ProjectType {get;}
    //    string StandardNamespace {get;}

    //    ITisClassInfo[] Classes {get;}
    //    ITisClassInfo GetClass(string sName);
    //    ITisClassInfo AddClass(string sName, string[] namespaces);
    //    string[] References { get;}

    //    ITisInvokeParams GetInvokeParams(ITisMethodInfo oMethodInfo);
    //    void Save();
    //    void Build(out bool bCompiled, out string sCompilerOutput);
    //    bool  IsDirty {get;}
    //    string GetMethodPrototype (string sMethodName, ITisMethodSignature oMethodSignature);
    //    bool SymbolsUpdated { get;set;}
    //    void Rebuild(out bool bCompiled, out string sCompilerOutput);
    //}

    //[Guid("EDC3208F-B824-4392-8087-DD11412DD4ED")]
    //public interface ITisClassInfo : ITisGenericEntity, ITisGenericMember
    //{
    //    string ModuleName {get;}
    //    ITisMethodInfo[] Methods {get;}
    //    ITisMethodInfo AddMethod(string sName, ITisMethodSignature oMethodSignature);
    //    ITisMethodInfo GetMethod(string sName);
    //}

    //[Guid("F2119435-157D-4cb8-BC6F-07690D3B20E7")]
    //public interface ITisMethodInfo : ITisGenericEntity
    //{
    //    ITisMethodSignature	Signature {get;set;}
    //}

    //[Guid("FFACCFDB-7B1A-4e6e-9AD2-8F522C1943CC")]
    //public interface ITisIDEController : ITisGenericEntity
    //{
    //    ITisIDEDriver[] All {get;}
    //    ITisIDEDriver[] GetIDEDrivers(string sProjectType);
    //    IList GetIDEDriversList(string sProjectType);
    //    ITisIDEDriver[] GetIDEDrivers();
    //    ITisIDEDriver GetIDEDriver(string sName);
    //}

    //[Guid("4F6B2DD0-5130-42ab-AB4E-61948E8DEC1B")]
    //public interface ITisIDEDriver : ITisGenericEntity
    //{
    //    string[] GetSupportedProjectTypes();
    //    void OpenSolution(ITisSolution oSolution);
    //    void OpenProject(ITisProjectInfo oProjectInfo);
    //    void Close(bool disposing);
    //    void Navigate(ITisMethodInfo oMethodInfo);
    //    void NavigateEx(int nMainWindowHandle, ITisMethodInfo oMethodInfo);
    //    bool SupportsProjectType(string sProjectType);
    //    bool IsAvailable { get; }
    //    Process RunIDEProcess(
    //        string sSolutionFileName, 
    //        IDEProcessSyncDelegate oSyncDelegate);
    //    void SyncCustomizationProject();
    //}

    //[Guid("633B86CC-20BE-4bb5-AC27-E8A286740AC1")]
    //public interface ITisProjectDebugInfo
    //{
    //    string StartProgram { get; set; }
    //    string StartArguments { get; set; }
    //    string StartWorkingDirectory { get; set; }
    //}

    #endregion
}
