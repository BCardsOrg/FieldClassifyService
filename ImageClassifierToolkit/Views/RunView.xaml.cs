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
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Views
{
    /// <summary>
    /// Interaction logic for RunView.xaml
    /// </summary>
    public partial class RunView : UserControl
    {
        MultiRunConfigurationViewModel m_multiRunConfigurationViewModel = new MultiRunConfigurationViewModel();

        public RunView()
        {
            InitializeComponent();
            parametersList.DataContext = m_multiRunConfigurationViewModel;
        }
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            var runCmd = new TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands.TrainRunCommand();

            runCmd.RunOnTrainData = chkbxRunOnTrainData.IsChecked == true;

            Shell.JumpToReportView();

            runCmd.Execute(null);

        }
        private void RunAndTune_Click(object sender, RoutedEventArgs e)
        {
            var runCmd = new TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands.TrainRunCommand();

            runCmd.TuneScale = true;

            runCmd.RunOnTrainData = chkbxRunOnTrainData.IsChecked == true;

            Shell.JumpToReportView();

            runCmd.Execute(null);

        }

        private void RunSet_Click(object sender, RoutedEventArgs e)
        {
            var runCmd = new TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands.TrainRunSetCommand();

            Shell.JumpToReportView();

            runCmd.Execute(m_multiRunConfigurationViewModel);
        }

        private void OnAll_Click(object sender, RoutedEventArgs e)
        {
            AppDataCenter.Singleton.UpdateFields(x => x.IsSelected = true);
        }

        private void OffAll_Click(object sender, RoutedEventArgs e)
        {
            AppDataCenter.Singleton.UpdateFields(x => x.IsSelected = false);
        }

    }
}
