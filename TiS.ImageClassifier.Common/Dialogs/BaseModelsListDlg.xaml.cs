using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TiS.Recognition.FieldClassifyService.Common.Dialogs
{
    /// <summary>
    /// Interaction logic for BaseModelsListDlg.xaml
    /// </summary>
    public partial class BaseModelsListDlg : Window
    {
        public BaseModelsListDlg()
        {
            InitializeComponent();
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                Type type = DataContext.GetType();

                PropertyInfo prop = type.GetProperty("ModelName");

                if (prop != null)
                {
                    prop.SetValue(DataContext, saveModelName.Text, null);
                }
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
