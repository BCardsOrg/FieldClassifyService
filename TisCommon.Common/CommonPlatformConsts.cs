using System;
using System.Runtime.InteropServices;
using System.Reflection;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon
{
	[ComVisible(false)]
	public class CommonPlatformConsts
	{
		public static string DEFAULT_IDENTITY = "Default";

		public const string MSSQL_INSTALL_FEATURE = "MSSQL";

		public const string STATION_ID_SWITCH				= "StationId";
		public const string STATION_APPID_SWITCH			= "ApplicationId";
		public const string STATION_UNATTENDED_SWITCH		= "Unattended";
		public const string STATION_PRECONFIGUREDID_SWITCH	= "PreconfiguredId";
		public const string STATION_STANDBY_SWITCH = "StandBy";
		public const string STATION_ANALYZER_SWITCH = "Analyzer";
		public const string STATION_ASSEMBLYNAME_SWITCH = "AssemblyName";
		
		public const string EMPTY_CAB = "empty.cab";

		public const string XML_EXTENSION = "XML";
        public const string CUSTOM_SOLUTION_DIR = "Customization";
        public const string CUSTOM_SOLUTION_EXTENSION = "TiSSLN";
        public const string CUSTOM_BIN_EXTENSION = "TiSBIN";
        public const string CUSTOM_BINARY_EXTENSION = "DLL";

		internal const string EFLOW_NODE_NAME_CTX_PARAM			 = "NodeName";
		internal const string EFLOW_STATION_INSTANCEID_CTX_PARAM = "StationInstanceId";
		internal const string EFLOW_APP_NAME_CTX_PARAM			 = "Application";
        internal const string EFLOW_PLATFORM_VERSION             = "PlatformVersion";

        public static readonly ModuleVersion BUILD82 = new ModuleVersion(3, 0, 82, 1);
        public static readonly ModuleVersion BUILD83 = new ModuleVersion(3, 0, 83, 1);
        public static readonly ModuleVersion BUILD91 = new ModuleVersion(3, 0, 91, 1);
        public static readonly ModuleVersion BUILD4_1 = new ModuleVersion(4, 0, 0, 1);
        public static readonly ModuleVersion BUILD4_5 = new ModuleVersion(4, 5, 0, 1);

        internal const int APP_RESOURCE_VERSIONS_TO_KEEP = 4;
	}
}
