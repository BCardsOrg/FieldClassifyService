using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
	public class SelectImagesFolderCommand : CommandBaseDialog
	{
        string m_folder;
		public SelectImagesFolderCommand() 
			: base(Shell.ShellDispatcher)
		{}

		protected override void ShowDialog()
		{
            
            m_folder = string.Empty;

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select images folder";
            dlg.IsFolderPicker = true;
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                m_folder = dlg.FileName;
            }
        }

		protected override void UpdateData()
		{
            if (string.IsNullOrEmpty(m_folder) == false)
            {
                var par = this.GetParameter();
                if (par != null && par is Action<string>)
                {
                    (par as Action<string>)(m_folder);
                }
            }
		}
	}
}
