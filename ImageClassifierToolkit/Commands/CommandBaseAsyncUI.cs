using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    /// <summary>
    /// Base class for async UI commands
    /// </summary>
    public abstract class CommandBaseAsyncUI: CommandBaseAsync 
    {
        #region Protected fields
        protected Dispatcher m_Dispatcher;
        
        #endregion

        #region Ctor

		public CommandBaseAsyncUI(Dispatcher dispatcher)
        {
            m_Dispatcher = dispatcher;
        }
        #endregion

        #region Protected methods
        protected override void DoWork(object parameter)
        {
            LoadData(parameter);

            m_Dispatcher.BeginInvoke((WaitCallback)delegate
            {
                UpdateView(parameter);

                ThreadPool.QueueUserWorkItem(UpdateData, parameter); 
            }, parameter); 
        }
        
        protected abstract void LoadData(object state);

        protected abstract void UpdateView(object state);

        protected abstract void UpdateData(object state);
        #endregion
    }
}
