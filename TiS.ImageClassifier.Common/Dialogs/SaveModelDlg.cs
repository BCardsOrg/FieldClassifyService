using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Recognition.FieldClassifyService.Common.Dialogs
{
    public class SaveModelDlg : BaseModelsListDlg
    {
        public SaveModelDlg(object viewModel)
            : base()
        {
            DataContext = viewModel;

            modelNameSection.Visibility = System.Windows.Visibility.Visible;
            
            Ok.Content = "Save";
        }
    }
}
