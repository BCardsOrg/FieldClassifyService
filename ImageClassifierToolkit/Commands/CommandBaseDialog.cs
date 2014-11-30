using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    public abstract class CommandBaseDialog: CommandBase
    {
		#region private member

		private Dispatcher m_Dispatcher;

        private object m_Parameter;

		#endregion

        public CommandBaseDialog(Dispatcher dispatcher)
        {
            m_Dispatcher = dispatcher;
        }
        
        public override void Execute(object parameter)
        {
			try
			{
				m_Parameter = parameter;
				m_Dispatcher.Invoke((WaitCallback)delegate
				{
					ShowDialog();
				}, this);

                ThreadPool.QueueUserWorkItem(delegate
                {
                    UpdateData();
                });
                //UpdateData();

			}
			finally
			{
			}
		}
    
        /// <summary>
        /// Show dialog before server update
        /// </summary>
        protected abstract void ShowDialog();
        /// <summary>
        /// Update Server asynchroniously 
        /// </summary>
        protected abstract void UpdateData();

        public object GetParameter()
        {
            return m_Parameter;
        }
    }
}
