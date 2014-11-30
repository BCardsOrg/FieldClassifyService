using System;

namespace TiS.Core.TisCommon.Customizations
{
	public class TisCustomizationConsts
	{
		public const string TIS_SOLUTION_TYPE_VBA    = "VBA";  
		public const string TIS_SOLUTION_TYPE_DN     = "DOTNET";  

		public const string TIS_PROJECT_TYPE_VBA     = "VBA";  
		public const string TIS_PROJECT_TYPE_VBNET   = "VBNET";  
		public const string TIS_PROJECT_TYPE_CSHARP  = "CSHARP";  

		public const string TIS_IDE_TYPE_VBA         = "VBA";  
		public const string TIS_IDE_TYPE_VS_2005     = "VS2005";
        public const string TIS_IDE_TYPE_VS_2008     = "VS2008";
        public const string TIS_IDE_TYPE_SHARPDEV    = "SHARPDEV";

        public const string MS_DTE_2005_REG_KEY      = "VisualStudio.DTE.8.0";
        public const string MS_DTE_2008_REG_KEY      = "VisualStudio.DTE.9.0";
                                                     
        public const string TIS_DEBUGGER_TYPE_VS_2005  = "VS2005";
        public const string TIS_DEBUGGER_TYPE_VS_2008  = "VS2008";
        public const string TIS_DEBUGGER_TYPE_SHARPDEV = "SHARPDEV";

        public const string TIS_INVOKE_TYPE_VBA      = "VBA";  
		public const string TIS_INVOKE_TYPE_WIN32DLL = "DLL";  
		public const string TIS_INVOKE_TYPE_COM      = "COM";  
		public const string TIS_INVOKE_TYPE_DOTNET   = "DOTNET";  

		public readonly static string[] TIS_STANDARD_PROJECT_REFERENCES = new string[] {
                                                                 "System", 
                                                                 "System.Windows.Forms", 
                                                                 "TiS.Core.TisCommon.Common",
		                                                         "TiS.Core.TisCommon.Client",
			                                                     "TiS.Core.Domain.Common",
			                                                     "TiS.Core.Domain.Client",
			                                                     "TiS.Core.Application.Common",
			                                                     "TiS.Core.Application.Client"};  

		public readonly static string[] TIS_STANDARD_PROJECT_IMPORTS = new string[] {
                                                              "System", 
                                                              "System.Windows.Forms", 
														  	  "TiS.Core.TisCommon.Common",
		                                                      "TiS.Core.TisCommon.Client",
			                                                  "TiS.Core.Domain.Common",
			                                                  "TiS.Core.Domain.Client",
			                                                  "TiS.Core.Application.Common",
			                                                  "TiS.Core.Application.Client"};

        public readonly static string[] TIS_STANDARD_PROJECT_TLB = new string[] {"TiS.Core.TisCommon.Common.TLB",
		                                                                         "TiS.Core.TisCommon.Client.TLB",
			                                                                     "TiS.Core.Domain.Common.TLB",
			                                                                     "TiS.Core.Domain.Client.TLB",
			                                                                     "TiS.Core.Application.Common.TLB",
			                                                                     "TiS.Core.Application.Client.TLB"};

        public readonly static string TIS_SOLUTION_DIRTY = Guid.NewGuid().ToString();
	}
}
