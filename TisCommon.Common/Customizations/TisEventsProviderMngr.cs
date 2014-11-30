using System;
using System.Collections;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Customizations
{
    public class TisEventsProviderManager : TisApplicationServicesProvider
	{
        public TisEventsProviderManager(
            string sApplicationName,
            ITisServicesHost oServicesHost)
            : base(oServicesHost, sApplicationName)
        {
        }

		#region ITisEventsProviderMngr Members

        public virtual ITisServiceInfo GetEventsProvider(string sKey)
		{
			return GetServiceInfo(sKey);
		}

		#endregion
	}
}
