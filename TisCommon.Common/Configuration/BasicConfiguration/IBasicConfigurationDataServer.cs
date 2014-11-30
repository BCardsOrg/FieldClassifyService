using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Configuration
{
    /// <summary>
    /// Provides server side configuration settings
    /// </summary>
    [Guid("AAEFC5C9-F2CF-4D29-8334-1C4937495E6F")]
    public interface IBasicConfigurationDataServer
    {
        /// <summary>
        /// Gets and sets the property indicating whether dynamic data will be saved in the file system or SQL server storage.
        /// </summary>
        /// <value>Default value is <b>false</b></value>
        bool UseSqlDynamicStorage { get; set; }
        /// <summary>
        /// Gets and sets the SQL server name
        /// </summary>
        /// <remark>Property should contain the full SQL server instance name.</remark>
        string LocalSQLServer { get; set; }
        /// <summary>
        /// Gets and sets the property indicating whether integrated security will be used for SQL connections.
        /// </summary>
        bool IntegratedSecurity { get; set; }
        /// <summary>
        /// Gets and sets the SQL server user name.
        /// </summary>
        /// <remarks>Relevant only if <see cref="IntegratedSecurity"/> is <b>false</b>.</remarks>
        string DBUserName { get; set; }

        /// <summary>
        /// Gets and sets the SQL server user password.
        /// </summary>
        /// <remarks>Relevant only if <see cref="IntegratedSecurity"/> is <b>false</b>.</remarks>
        string DBPassword { get; set; }

    }

}


