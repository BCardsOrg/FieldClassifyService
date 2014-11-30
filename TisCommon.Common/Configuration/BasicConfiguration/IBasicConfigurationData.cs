using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Configuration
{
	/// <summary>
	/// Provides the client side configuration settings
	/// </summary>
    [Guid("377617D1-5C47-4938-B809-8326BDC2ED63")]
	public interface IBasicConfigurationData 
	{
        /// <summary>
        /// Gets and sets the path to eFLOW data.
        /// </summary>
		string					eFlowDataPath	     { get; set; }
		/// <summary>
        /// Gets and sets the path to eFLOW client installation. 
        /// </summary>
		string					eFlowInstallPath     { get; set; }
        /// <summary>
        /// Gets the path to eFLOW client binaries directory.
        /// </summary>
        /// <remarks>Used internally</remarks>
        string                  eFlowBinPath { get; }
        /// <summary>
        /// Gets and sets the path to eFLOW OCR engines installation.
        /// </summary>
		string				eFlowOCRsPath	     { get; set; }

        /// <summary>
        /// Gets the path to eFLOW temp directory
        /// </summary>
        /// <remarks>Used internally</remarks>
        string                 eFlowTempPath { get; }
        /// <summary>
        /// Gets and sets the path to eFLOW localization files directory.
        /// </summary>
		string					eFlowLangPath	     { get; set; }
        /// <summary>
        /// Gets and sets the path to eFLOW DB Engines installation.
        /// </summary>
		string					eFlowDBEnginesPath	 { get; set; }
        /// <summary>
        /// if true save the user login name & password in local machine, so next time login to User & Password will be automatic
        /// </summary>
        bool                    SaveUserAuthentication { get; set; }

        /// <summary>
        /// Gets and sets the server machine name
        /// </summary>
        string                  ServerMachineName { get; set; }
        /// <summary>
        /// Gets and sets the local machine name
        /// </summary>
        /// <remarks>In the standalone installation the server and local machine names are identical.</remarks>
        string                  LocalMachineName { get; set; }

        /// <summary>
        /// Gets and sets the communication protocol.
        /// </summary>
        /// <value>Default value is <b>http</b></value>
        string                 CommunicationProtocol { get; set; }

        /// <summary>
        /// Gets and sets the communication port used by the communication protocol defined by the <see cref="CommunicationProtocol"/> property.
        /// </summary>
        /// <value>Default value is <b>55222</b></value> 
        int                    CommunicationPort { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether eFLOW will run in demo mode.
		/// </summary>
		bool				DemoMode { get; set; }

    }
}
