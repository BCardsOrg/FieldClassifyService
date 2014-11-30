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
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Views
{
	/// <summary>
	/// Interaction logic for ReportView.xaml
	/// </summary>
	public partial class AnalyseView : UserControl
	{
        public AnalyseView()
		{
			InitializeComponent();
            var viewmodel = DataContext as AnalyseViewModel;
         //   graphlister.ItemsSource = viewmodel.EntireGraphs;
            
		}

        private void SaveModel_Click(object sender, RoutedEventArgs e)
        {
            var cmd = new SaveModelDataCommand();

            cmd.Execute(null);
        }

        private void Best_Click(object sender, RoutedEventArgs e)
        {
           // var cmd = new SelectBestReportDataCommand();
         //   AppDataCenter.Singleton.NotifyChange(NotifyGroup.Progress);
           // cmd.Execute(null);
        }

        private void leftRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var viewmodel = DataContext as AnalyseViewModel;
            viewmodel.LeftFeature = ((sender as FrameworkElement).DataContext as FeatureSelectModel).SerialID;


        }

        private void rightRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var viewmodel = DataContext as AnalyseViewModel;
            viewmodel.RightFeature = ((sender as FrameworkElement).DataContext as FeatureSelectModel).SerialID;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var viewmodel = DataContext as AnalyseViewModel;
            viewmodel.UpdateScatter();
           // viewmodel
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var viewmodel = DataContext as AnalyseViewModel;
            viewmodel.UpdateScatter();
        }

    }
}
