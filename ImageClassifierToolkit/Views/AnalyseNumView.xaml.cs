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
using System.Dynamic;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using System.ComponentModel;
using TiS.Recognition.FieldClassifyService.API_GoldenData;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Views
{
	/// <summary>
	/// Interaction logic for ReportView.xaml
	/// </summary>
	public partial class AnalyseNumView : UserControl
	{
        public AnalyseNumView()
		{
			InitializeComponent();
        //    FeatureHolder9.Items.SortDescriptions.Add(
        //new SortDescription("Item6", ListSortDirection.Descending));

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
            var viewmodel = DataContext as AnalyseNumViewModel;
         //   viewmodel.LeftFeature = ((sender as FrameworkElement).DataContext as FeatureSelectModel).SerialID;


        }

        private void rightRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var viewmodel = DataContext as AnalyseNumViewModel;
          //  viewmodel.RightFeature = ((sender as FrameworkElement).DataContext as FeatureSelectModel).SerialID;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var viewmodel = DataContext as AnalyseNumViewModel;
            viewmodel.ChosenField = ((Tuple<string,double,double>)((sender as FrameworkElement).DataContext )).Item1;
            AppDataCenter.Singleton.NotifyChange(NotifyGroup.StatisticData);
        }

       

        private void FeatureHolder4_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem != null)
            {
                var viewmodel = DataContext as AnalyseNumViewModel;
                viewmodel.ChosenField = ((Tuple<string, double, double>)(sender as DataGrid).SelectedItem).Item1;
                viewmodel.ChosenFieldReport = null;
                AppDataCenter.Singleton.NotifyChange(NotifyGroup.StatisticData);
            }
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewmodel = DataContext as AnalyseNumViewModel;
            viewmodel.ChosenFieldReport = (FieldReportItem)((sender as DataGrid).SelectedItem);
            AppDataCenter.Singleton.NotifyChange(NotifyGroup.StatisticData);
        }

        private void FeatureHolder9_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             var viewmodel = DataContext as AnalyseNumViewModel;
             viewmodel.ChosenResult = ((Tuple<string, double, double, double, double, double>)(sender as DataGrid).SelectedItem);
             AppDataCenter.Singleton.NotifyChange(NotifyGroup.StatisticData);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double newGrade;
            if(!double.TryParse(TextBoxGrade.Text,out newGrade)) return;

            var viewmodel = DataContext as AnalyseNumViewModel;

            int featureIndex = AppDataCenter.Singleton.ChosenFeatures.IndexOf(x => x.Name == viewmodel.ChosenResult.Item1);

            int ChosenField = AppDataCenter.Singleton.EntireModelStats.fieldList.First(a => a.name == viewmodel.ChosenField).index;
            double[] NewFeatureLIst = viewmodel.ChosenFieldReport.ExpectedField.Features.Select((a, i) => i == featureIndex ? newGrade : a).ToArray();
            double[] ConfidanceOut;
            ClassifierService.Service.GetDescition(NewFeatureLIst, out ConfidanceOut);
           // ConfidanceOut = ClassifierService.Service.NormalizaedConfidence(ConfidanceOut);
            viewmodel.NewConfidanceResult = Math.Round(ConfidanceOut[ChosenField] * 100,1);
            AppDataCenter.Singleton.NotifyChange(NotifyGroup.StatisticData);

        }
        private void FeatureList1_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
          /*  var viewmodel = DataContext as AnalyseNumViewModel;
            viewmodel.chosenFieldFromFeatures = null;
            viewmodel.ChosenField = null;
            viewmodel.SelectedFeature = ((KeyValuePair<string, double>)(sender as DataGrid).SelectedItem);
            viewmodel.FieldsFeatureGradeList = null;*/
            var viewmodel = DataContext as AnalyseNumViewModel;
            viewmodel.viewmodeltub2.FieldsFeatureGradeList = null;
        }

        private void FeatureList1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*  var viewmodel = DataContext as AnalyseNumViewModel;
              if ((sender as DataGrid).SelectedItem == null) return;
             // KeyValuePair<string, double> selectedFeatureTemp = viewmodel.SelectedFeature;
              viewmodel.chosenFieldFromFeatures = ((KeyValuePair<string, double>)((sender as DataGrid).SelectedItem)).Key;
             // viewmodel.SelectedFeature = selectedFeatureTemp;
        //    AppDataCenter.Singleton.NotifyChange(NotifyGroup.StatisticData);*/

        }

        private void wordsCandidatesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as AnalyseNumViewModel;

            viewModel.HighlightWordCandidate(e.AddedItems.Cast<CandidateData>());
        }

      

    }
}
