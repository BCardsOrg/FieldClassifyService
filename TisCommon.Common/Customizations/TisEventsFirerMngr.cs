using System;
using System.Collections;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Customizations
{
	public interface ITisEventsFirerMngr
	{
		ITisEventFirer GetEventsFirer (string sEventSourceKey, string sEventsFirerKey);
	}

    public class TisEventsFirerManager : TisApplicationServicesProvider, ITisEventsFirerMngr
	{
        public TisEventsFirerManager(
            string sApplicationName,
            ITisServicesHost oServicesHost)
            : base(oServicesHost, sApplicationName)
        {
        }

		#region ITisEventsFirerMngr Members

		public ITisEventFirer GetEventsFirer(string sEventSourceKey, string sEventsFirerKey)
		{
			return (ITisEventFirer)GetService (sEventsFirerKey, sEventSourceKey);
		}

		#endregion
	}
}
