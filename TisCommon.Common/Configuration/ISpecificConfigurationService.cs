using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Configuration
{
    [Guid("C4C077D9-1196-4A47-ACA2-A7445465FC92")]
    [ServiceContract(Namespace = "http://www.topimagesystems.com/Core/TisCommon/Configuration/SpecificConfigurationService")]
    public interface ISpecificConfigurationService    
    {
        /// <summary>
        /// Returns contents of the specific configuration section.
        /// </summary>
        [OperationContract]
        string LoadConfigurationData(string sectionName);

        /// <summary>
        /// Updates contents of the specific configuration section
        /// </summary>
        [OperationContract]
        void SaveConfigurationData(string sectionName, string data);

        /// <summary>
        /// Removes the specific section from the configuration file.
        /// </summary>
        [OperationContract]
        void RemoveConfigurationSection(string stationName);

        /// <summary>
        /// Renames the configuration section
        /// </summary>
        [OperationContract]
        void RenameConfigurationSection(string oldStationName, string newStationName);
    }
}
