using System.Collections.Generic;

namespace TiS.Core.TisCommon.Services
{
    public class TisInternalServicesSchema : TisServicesSchemaCreator, ITisServicesSchemaCollection
    {
        private readonly ITisServiceInfo[] Services = 
        {
        };

        #region ITisServicesSchemaCollection Members

        public virtual List<ITisServiceInfo> GetServices()
        {
            return new List<ITisServiceInfo>(Services);
        }

        #endregion
    }
}
