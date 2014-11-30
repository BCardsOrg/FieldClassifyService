using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace TiS.Core.TisCommon.Services.Web
{
    #region WebSessionData

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisWebSessionData
    {
        public TisWebSessionData(string userName, byte[] permissionToken)
        {
            UserName = userName;
            PermissionToken = permissionToken;
        }

        public TisWebSessionData(string userName, byte[] permissionToken, string stationNode)
            : this(userName, permissionToken)
        {
            StationNode = stationNode;
        }

        public TisWebSessionData(
            string userName,
            byte[] permissionToken,
            string stationName,
            string stationId,
            string stationNode,
            string stationApplicationName)
            : this(userName, permissionToken, stationNode)
        {
            UserName = userName;
            StationName = stationName;
            StationId = stationId;
            ApplicationName = stationApplicationName;
        }

        [DataMember]
        public byte[] PermissionToken { get; set; }

        [DataMember]
        public string StationId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string StationNode { get; set; }

        [DataMember]
        public string StationName { get; set; }

        [DataMember]
        public string ApplicationName { get; set; }
    }

    #endregion
}
