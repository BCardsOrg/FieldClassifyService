using System;
using System.Collections.Generic;
using TiS.Core.TisCommon.Customizations.MethodInvokers.Managed;
using TiS.Core.TisCommon.Customizations.MethodBrowsers;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Customizations
{
	#region TisInvokeTypeBase

	abstract public class TisInvokeTypeBase : ITisInvokeType
	{
        Dictionary<string, ITisEventInvoker> m_oEventInvokers =
            new Dictionary<string, ITisEventInvoker>();

		protected TisInvokeTypeMiscellaneous m_oMiscellaneous = null;
		
		#region ITisInvokeType Members

		abstract public string TypeName {get;}
		abstract public string[] GetBinTypes();

		public virtual ITisEventInvoker GetEventsInvoker()
		{
            if (m_oEventInvokers.ContainsKey(TypeName))
            {
                ITisEventInvoker oEventInvoker;

                m_oEventInvokers.TryGetValue(TypeName, out oEventInvoker);

                return oEventInvoker;
            }

			if (OnGetInvoker != null)
			{
				OnGetInvoker (TypeName, Miscellaneous);
			}

			return null;
		}

		public virtual TisInvokeTypeMiscellaneous Miscellaneous
		{
			get
			{
				if (m_oMiscellaneous == null)
				{
					m_oMiscellaneous = new TisInvokeTypeMiscellaneous ();
				}

				return m_oMiscellaneous;
			}
		}

		abstract public ITisEventsBrowser GetEventsBrowser(ITisInvokeParams oInvokeParams);
		abstract public ITisEventsExplorer GetEventsExplorer();

		public event TisGetInvokerDelegate OnGetInvoker;

        protected void AddEventInvoker(ITisEventInvoker oEventInvoker)
        {
            if (!m_oEventInvokers.ContainsKey(TypeName))
            {
                m_oEventInvokers.Add(TypeName, oEventInvoker);
            }
        }

		#endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            foreach (ITisEventInvoker eventInvoker in m_oEventInvokers.Values)
            {
                eventInvoker.Dispose();
            }

            m_oEventInvokers.Clear();
        }

        #endregion
    }

	#endregion

	#region TisInvokeTypeMiscellaneous

	public class TisInvokeTypeMiscellaneous
	{
        private string m_sDescription = String.Empty;

        public virtual string Description
        {
            get
            {
                return m_sDescription;
            }
        }
    }

	#endregion

	#region TisDNInvokeType

	public class TisDNInvokeType : TisInvokeTypeBase
	{
        private ICustomAssemblyResolver m_oAsseblyResolver;
        private ITisEventsExplorer m_oEventsExplorer;

        public TisDNInvokeType(
            ICustomAssemblyResolver oAsseblyResolver)
        {
            m_oAsseblyResolver = oAsseblyResolver;
        }

        public override void Dispose()
        {
            if (m_oAsseblyResolver != null)
            {
                m_oAsseblyResolver.Dispose();
                m_oAsseblyResolver = null;
            }

            if(m_oEventsExplorer != null)
            {
                m_oEventsExplorer.Dispose();
                m_oEventsExplorer = null;
            }

            base.Dispose();
        }

		#region ITisInvokeType Members

		public override string TypeName
		{
			get
			{
				return TisCustomizationConsts.TIS_INVOKE_TYPE_DOTNET;
			}
		}

		public override string[] GetBinTypes()
		{
			return new string[] {"DLL", "PDB"};
		}

		public override ITisEventInvoker GetEventsInvoker()
		{
            ITisEventInvoker oEventInvoker = 
                base.GetEventsInvoker();

            if (oEventInvoker == null)
            {
                oEventInvoker = new TisDNEventInvoker(
                    m_oAsseblyResolver.CustomizationDir,
                    ((TisDNInvokeTypeMiscellaneous)m_oMiscellaneous).Debuggers,
                    m_oAsseblyResolver);

                AddEventInvoker(oEventInvoker);
            }

            return oEventInvoker;
		}

		public override ITisEventsBrowser GetEventsBrowser(ITisInvokeParams oInvokeParams)
		{
			return null;
		}

		public override ITisEventsExplorer GetEventsExplorer()
		{
            if (m_oEventsExplorer == null)
            {
                m_oEventsExplorer = new TisEventsExplorer(EXPLORER_TYPE.DOTNET, m_oAsseblyResolver);
            }

            return m_oEventsExplorer;
		}

        public override TisInvokeTypeMiscellaneous Miscellaneous
        {
            get
            {
                if (m_oMiscellaneous == null)
                {
                    m_oMiscellaneous = new TisDNInvokeTypeMiscellaneous();
                }

                return m_oMiscellaneous;
            }
        }

        #endregion
	}
    #endregion

    #region TisDNInvokeTypeMiscellaneous

    public class TisDNInvokeTypeMiscellaneous : TisInvokeTypeMiscellaneous
    {
        private List<ITisDebugger> m_oDebuggers;

        #region ITisDNInvokeTypeMiscellaneous Members

        public List<ITisDebugger> Debuggers 
        {
            get
            {
                return m_oDebuggers;
            }
            set
            {
                m_oDebuggers = value;
            }
        }

        public override string Description
        {
            get
            {
                return ".NET";
            }
        }

        #endregion
    }

    #endregion
}
