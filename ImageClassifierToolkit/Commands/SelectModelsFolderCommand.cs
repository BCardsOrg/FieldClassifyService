using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    class SelectModelsFolderCommand : CommandBaseDialog
	{
        public SelectModelsFolderCommand() 
			: base(Shell.ShellDispatcher)
		{}

		protected override void ShowDialog()
		{
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var par = this.GetParameter();
                    if (par != null && par is Action<string>)
                    {
                        (par as Action<string>)( dlg.SelectedPath );
                    }
                }
            }
        }

		protected override void UpdateData()
		{
		}
	}
}
