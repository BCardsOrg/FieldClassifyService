using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
	delegate void ExceptionCallback(Exception ex);

	/// <summary>
    /// Base class for async commands in system
    /// </summary>
    public abstract class CommandBaseAsync: CommandBase
	{
		void MyExceptionCallback(Exception ex)
		{
			throw ex; // Handle/re-throw if necessary
		}

		#region private member
		#endregion
		#region Public Events
		public event EventHandler Executed;
        #endregion

        #region Ctor
        #endregion

        #region Public methods
        public override void Execute(object parameter)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
				try
				{
					try
					{
						DoWork(parameter);

						if (Executed != null)
						{
							Executed(this, EventArgs.Empty);
						}
					}
					finally
					{
					}

				}
				catch (Exception e)
				{
					Shell.ShellDispatcher.BeginInvoke(new ExceptionCallback(MyExceptionCallback), e); 
				}
			});
        }
        #endregion

        #region Protected methods
        protected abstract void DoWork(object parameter);
        #endregion
    }
}
