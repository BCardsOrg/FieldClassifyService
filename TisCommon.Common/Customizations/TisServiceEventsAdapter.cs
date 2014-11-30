
namespace TiS.Core.TisCommon.Customizations
{
	public delegate object EventInterceptorDelegate (
	    ITisEventsManager oEventsManager, 
	    object oEventSource,
        object oEventBindingKey,
        string sEventName, 
	    ref object[] InOutParams);

	internal class TisServiceEventsAdapter
	{
        public TisServiceEventsAdapter()
        {
            OnEventInterception += new EventInterceptorDelegate(OnEventInterceptionHandler);
        }

        public EventInterceptorDelegate EventInterceptorDelegate
        {
            get
            {
                return OnEventInterception;
            }
        }

        public event EventInterceptorDelegate OnEventInterception;

		// Service events interceptor

        public object OnEventInterceptionHandler(
            ITisEventsManager oEventsManager,
            object oEventSource,
            object oEventBindingKey,
            string sEventName,
            ref object[] InOutParams)
        {
            return ((TisEventsManager)oEventsManager).FireEvent(
                oEventSource,
                oEventBindingKey,
                sEventName,
                ref InOutParams);
        }
	}
}
