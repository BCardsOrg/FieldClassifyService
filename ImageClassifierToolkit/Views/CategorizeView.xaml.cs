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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Views
{
    /// <summary>
    /// Interaction logic for CategorizeView.xaml
    /// </summary>
    public partial class CategorizeView : UserControl
    {
        public CategorizeView()
        {
            InitializeComponent();
        }
        private void SetGoldClass_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CategorizeViewModel;

            vm.SetGoldClass(vm.SelectedGoldClassName);
        }
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CategorizeViewModel;

            var runCmd = new AutoCategorizationCommand();

            runCmd.ClassifierField = vm.SelectedClassifierField;

            runCmd.Execute(null);
        }

        private void SetClassPerFolder_Click(object sender, RoutedEventArgs e)
        {
            var setCmd = new SetClassPerFolderCommand();

            setCmd.Execute(null);
        }
    }
}
