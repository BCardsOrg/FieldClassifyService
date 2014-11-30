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
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Views
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class HistoryView : UserControl
    {
        public HistoryView()
        {
            InitializeComponent();
        }

        private void LoadModel_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as HistoryViewModel;

            AppDataCenter.Singleton.LoadModel(vm.ModelsViewModel.SelectedModel.Name);
        }

      /*  private void SaveToApp_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as HistoryViewModel;

            if ( vm.ModelsViewModel.SelectedModel != null )
            {

                string modelName = vm.ModelsViewModel.SelectedModel.Name;

                var cmd = new SaveToApplicationCommand();

                cmd.Execute(modelName);
            }
        }*/

       /* private void LoadFromApp_Click(object sender, RoutedEventArgs e)
        {
            var cmd = new LoadFromApplicationCommand();

            cmd.Execute(null);
        }*/
    }
}
