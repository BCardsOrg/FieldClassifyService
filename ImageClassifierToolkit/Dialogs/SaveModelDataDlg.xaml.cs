using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Dialogs
{
    /// <summary>
    /// Interaction logic for SaveModelDataDlg.xaml
    /// </summary>
    public partial class SaveModelDataDlg : Window
    {
        public SaveModelDataDlg()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        public static readonly DependencyProperty ModelNameProperty =
            DependencyProperty.Register("ModelName", typeof(string),
            typeof(Shell),
            new UIPropertyMetadata(null));
        public string ModelName
        {
            get
            {
                return (string)GetValue(ModelNameProperty);
            }
            set
            {
                SetValue(ModelNameProperty, value);
            }
        }




    }
}
