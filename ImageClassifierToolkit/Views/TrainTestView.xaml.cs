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
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Views
{
    /// <summary>
    /// Interaction logic for TrainTestView.xaml
    /// </summary>
    public partial class TrainTestView : UserControl
    {
        public TrainTestView()
        {
            InitializeComponent();
        }
        private void SelectTrainData_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as TrainTestViewModel;
            var runCmd = new SelectTrainDataCommand();

            runCmd.MixClasses = vm.IsMixTrain;

            runCmd.Execute(Convert.ToDouble(trainRate.Text) / 100d);
        }

        private void ToggleTrain_Click(object sender, RoutedEventArgs e)
        {
            AppDataCenter.Singleton.ToggleTrainPages(
                PagesViewModel.Pages.SelectMany( x => x.Pages ).Where( x => x.IsSelected == true )
                                    .Select(x => AppDataCenter.Singleton.SetupData.Pages.First(y => x.FileName == y.Setup.FileName && x.PageOrder == y.Setup.PageNo)));
        }
    }
}
