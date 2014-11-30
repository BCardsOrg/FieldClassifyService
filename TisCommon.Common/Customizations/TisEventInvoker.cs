using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TiS.Core.TisCommon.Customizations.MethodInvokers.Managed;

namespace TiS.Core.TisCommon.Customizations
{
	#region TisEventInvoker

	abstract internal class TisEventInvoker : ITisEventInvoker
	{
		private bool m_bDebugMode;
        private List<ITisDebugger> m_oAvailableDebuggers;

		#region ITisEventInvoker Members

		public virtual object Invoke (ITisInvokeParams oInvokeParams, ITisEventParams oEventParams, ref object[] InOutParams)
		{
			return null;
		}

		public virtual bool DebugMode
		{
			get
			{
				return m_bDebugMode;
			}
			set
			{
				m_bDebugMode = value;
			}
		}

        internal List<ITisDebugger> AvailableDebuggers
        {
            get
            {
                return m_oAvailableDebuggers;
            }
            set
            {
                m_oAvailableDebuggers = value;

                if (m_oAvailableDebuggers != null)
                {
                    for (int i = m_oAvailableDebuggers.Count - 1; i >= 0; i--)
                    {
                        ITisDebugger oDebugger = m_oAvailableDebuggers[i];

                        if (!oDebugger.IsAvailable)
                        {
                            m_oAvailableDebuggers.Remove(oDebugger);
                        }
                    }
                }
            }
        }

        protected virtual void ValidateInvoker(ITisInvokeParams oInvokeParams, string sInvokeType)
		{
			if (oInvokeParams.InvokeType != sInvokeType)
			{
				throw new TisException ("Invalid invoke type [{0}]", oInvokeParams.InvokeType);
			}
		}

		#endregion


        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
	#endregion

	#region TisDNEventInvoker

	internal class TisDNEventInvoker : TisEventInvoker
	{
        private ManagedMethodInvoker m_oInvoker;

        public TisDNEventInvoker(
            string sCustomizationDir,
            List<ITisDebugger> InstalledDebuggers,
            ICustomAssemblyResolver oAssemblyResolver)
        {
            oAssemblyResolver.CustomizationDir = sCustomizationDir;

            AvailableDebuggers = InstalledDebuggers;

            m_oInvoker = new ManagedMethodInvoker(oAssemblyResolver.AssemblyResolveHandler);
        }

        #region ITisEventInvoker Members

        public override bool DebugMode
        {
            get
            {
                return base.DebugMode;
            }
            set
            {
                base.DebugMode = value;
            }
        }

        public override object Invoke(ITisInvokeParams oInvokeParams, ITisEventParams oEventParams, ref object[] InOutParams)
		{
			ValidateInvoker (oInvokeParams, TisCustomizationConsts.TIS_INVOKE_TYPE_DOTNET);

			return m_oInvoker.InvokeMethod (
				oInvokeParams.ModuleName,
				oInvokeParams.ClassName,
				oInvokeParams.MethodName,
				ref InOutParams);
		}

		#endregion

        public override void Dispose()
        {
            m_oInvoker.Dispose();

            m_oInvoker = null;

            base.Dispose();
        }
	}

	#endregion
}
