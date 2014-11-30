using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    /// <summary>
    /// Base class for all commands 
    /// </summary>
    public abstract class CommandBase: ICommand
    {
        #region Private fields
        #endregion

		#region Public fields

		public string Name { get; set; }

		#endregion

		#region Ctor
        #endregion

        #region Public methods

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public abstract void Execute(object parameter);

        #endregion

        #region Protected methods
        protected void RaiseCanExecuteChanged() 
        {
            if (CanExecuteChanged != null) 
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
