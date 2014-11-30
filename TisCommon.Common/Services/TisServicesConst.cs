using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Services.Web;

namespace TiS.Core.TisCommon.Services
{
	[ComVisible(false)]
	public class TisServicesConst
	{
		public const string SystemApplication = "System";
		public const string SERVER_ROLE		  = "Server";
		public const string UNNAMED_INSTANCE  = "$Unnamed";

        public const double ACCESS_COST_IN_PROCESS = 0;
        public const double ACCESS_COST_IN_MACHINE = 1;
        public const double ACCESS_COST_INTER_SITE = 30;

        public static string[] NO_ROLES_REQUIRED = EmptyArrays.StringArray;
        public static string[] SERVER_REQUIRED = new string[] { TisServicesConst.SERVER_ROLE };

        public const int DEFAULT_COMMUNICATION_PORT = 55222;

        public const string DEFAULT_COMMUNICATION_PROTOCOL = TisWebServiceExtensions.COMMUNICATION_PROTOCOL_WS_HTTP;

        public const string SERVICE_REGISTRY_PROVIDER_TYPE_NAME = "TiS.Core.TisCommon.Services.TisServicesSchema,TiS.Core.TisCommon.Common";

        public const string CLAIMS_SERVICE_WINDOWS = "ClaimsServiceWindows";
        public const string CLAIMS_SERVICE_USER    = "ClaimsServiceUser";

        public const string IIS_PROCESS_FILE_NAME = "w3wp.exe";
        public const string IIS_PROCESS_NAME = "w3wp";
        public const string VS_PROCESS_NAME  = "devenv";
        public static readonly string[] VS_TEST_PROCESS_NAMES = new string[] { "QTAgent32","vstest.executionengine.x86" };
        public const string CONFIG_FILE_EXT  = ".config";
    }
}
