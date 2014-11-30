using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Recognition.FieldClassifyService.Common.Dialogs
{
    public class SelectModelDlg : BaseModelsListDlg
    {
        public SelectModelDlg(object viewModel) : base()
        {
            DataContext = viewModel;

            modelNameSection.Visibility = System.Windows.Visibility.Collapsed;
            
            Ok.Content = "Select";
        }
    }
}
