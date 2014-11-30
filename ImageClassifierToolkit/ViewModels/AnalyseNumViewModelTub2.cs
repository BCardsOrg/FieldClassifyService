using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Statistic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using OxyPlot;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls.OxyPlot;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{

    public class AnalyseNumViewModelTub2 : SectionViewModel
    {
        public AnalyseNumViewModelTub2()
        {

          /*  RegisterProperty("FeatureGrades", NotifyGroup.StatisticData);
            RegisterProperty("FieldFeatureGrades", NotifyGroup.StatisticData);
            RegisterProperty("FalsePositiveResults", NotifyGroup.StatisticData);
       
            RegisterProperty("FPAndExpectedGrade", NotifyGroup.StatisticData);
            RegisterProperty("ChosenFieldReport", NotifyGroup.StatisticData);
            RegisterProperty("ChosenResult", NotifyGroup.StatisticData);

            RegisterProperty("NewConfidanceResult", NotifyGroup.StatisticData);*/
         
            

            
            
            

        }

        public override bool Enable
        {
            get
            {
                return AppDataCenter.Singleton.IsRunValid;
            }
        }
        public override bool IsValid
        {
            get
            {
                return true;
            }
        }





      KeyValuePair<string,double> m_chosenField;
      public KeyValuePair<string, double> ChosenField
      {
          get
          {
              return m_chosenField;
          }
          set
          {
              if ( OnChange(ref m_chosenField, value, "ChosenField" ) == true )
              {
                 // SelectedFeature = default( KeyValuePair<string, double> );
                  UpdatePlot();
              }
          }
      }
      public FieldReportItem ChosenFieldReport { get; set; }
      public Tuple<string, double, double, double, double, double> ChosenResult { get; set; }

      public double NewConfidanceResult { get; set; }



      public ObservableCollection<KeyValuePair<string, double>> FeatureGrades
        {
            get
            {
                ObservableCollection<KeyValuePair<string, double>> result = new ObservableCollection<KeyValuePair<string, double>>();
           AppDataCenter.Singleton.ChosenFeatures.Select((b,i)=>new{feature = b,index = i}).ToList().ForEach(a=>{
               result.Add(new KeyValuePair<string, double>(a.feature.Name, AppDataCenter.Singleton.EntireModelStats.GetFeatureGrade(a.index)));
            });
           return (new ObservableCollection<KeyValuePair<string, double>>(result.OrderBy(a => a.Value).ToList()));
            }
            set
            {
                
              //  OnChange(FeatureGrades, value, x => AppDataCenter.Singleton.FeatureGrades = x);
            }
        }

        KeyValuePair<string, double> m_selectedFeature;
        public KeyValuePair<string, double> SelectedFeature
        {
            get
            {
                return m_selectedFeature;
            }
            set
            {
                if (OnChange(ref m_selectedFeature, value, "SelectedFeature") == true)
                {
                    m_chosenField = default(KeyValuePair<string, double>);
                  //  UpdatePlot();
                   
                }
            }
        }

       


        #region fieldHistogram
        ColumnSeries m_featureHistogramPlotPoints;
        ColumnSeries m_featureHistogramPlotPointsOther;
        CategoryAxis m_xAxis;
        LinearAxis m_yAxis;
        private void UpdatePlot()
        {
            if (ChosenField.Equals(default(KeyValuePair<string,double>))) return;
            if (m_featureHistogram == null)
            {
                m_featureHistogram = new PlotModel();

                m_featureHistogram.PlotMargins = new OxyThickness(-10, -10, 0, 0);
                m_featureHistogram.PlotAreaBorderThickness = 1;
                m_featureHistogram.LegendTextColor = OxyColor.FromRGB(0x92, 0x92, 0x92);
                m_featureHistogram.LegendMargin = 0;
                m_featureHistogram.LegendSymbolMargin = 0;
                m_featureHistogram.LegendBorderThickness = 0;
                m_featureHistogram.LegendBackground = OxyColors.Transparent;
                m_featureHistogram.LegendOrientation = LegendOrientation.Horizontal;
                m_featureHistogram.LegendPlacement = LegendPlacement.Inside;
                m_featureHistogram.LegendPadding = 0;

                m_xAxis = new CategoryAxis();
                m_xAxis.Position = AxisPosition.Bottom;
                m_xAxis.MajorGridlineStyle = LineStyle.Solid;
                m_xAxis.MajorGridlineThickness = 1;
                m_xAxis.MinorGridlineThickness = 0;
                m_xAxis.TickStyle = TickStyle.None;
                m_xAxis.IsPanEnabled = false;
                m_xAxis.IsZoomEnabled = false;
                m_featureHistogram.Axes.Add(m_xAxis);

                m_yAxis = new LinearAxis();
                m_yAxis.Position = AxisPosition.Left;
                m_featureHistogram.Axes.Add(m_yAxis);

                m_featureHistogramPlotPoints = new ColumnSeries();
                m_featureHistogram.Series.Add(m_featureHistogramPlotPoints);

                m_featureHistogramPlotPointsOther = new ColumnSeries();
                m_featureHistogram.Series.Add(m_featureHistogramPlotPointsOther);
            }

            m_featureHistogramPlotPoints.Items.Clear();
            m_featureHistogramPlotPointsOther.Items.Clear();

            var field = AppDataCenter.Singleton.EntireModelStats.fieldList.First(b => b.name == ChosenField.Key);
            var fields = AppDataCenter.Singleton.EntireModelStats.fieldList.Where(b => b.name != ChosenField.Key).ToList();
            fields.Add(AppDataCenter.Singleton.EntireModelStats.notAName);

            var featureId = AppDataCenter.Singleton.ChosenFeatures.FindIndex(x => x.Name == SelectedFeature.Key);

            if (featureId >= 0)
            {
                var histogram = AppDataCenter.Singleton.EntireModelStats.GetFeatureFieldHistogram(10, featureId, field);

                var histogramGraph = histogram.GetHistogramGraphNormalized();

                var histogramOther = AppDataCenter.Singleton.EntireModelStats.GetFeatureFieldHistogram(10, featureId, fields);

                var histogramGraphIther = histogramOther.GetHistogramGraphNormalized();

                if (histogramGraph.Keys.Last() > 50)
                {
                    m_xAxis.Labels = histogramGraph.Keys.Select(x => x.ToString("F0")).ToArray();
                }
                else if (histogramGraph.Keys.Last() > 5)
                {
                    m_xAxis.Labels = histogramGraph.Keys.Select(x => x.ToString("F1")).ToArray();
                }
                else
                {
                    m_xAxis.Labels = histogramGraph.Keys.Select(x => x.ToString("F2")).ToArray();
                }

                foreach (var histogramData in histogramGraph)
                {
                    m_featureHistogramPlotPoints.Items.Add(new ColumnItem(histogramData.Value));

                }

                foreach (var histogramData in histogramGraphIther)
                {
                    m_featureHistogramPlotPointsOther.Items.Add(new ColumnItem(histogramData.Value,-1,OxyColors.Blue));

                }
            }

            if (m_featureHistogram.PlotControl != null)
            {
                m_featureHistogram.PlotControl.RefreshPlot();
            }

            OnPropertyChanged("FeatureHistogram");
        }


        PlotModel m_featureHistogram;
        public PlotModel FeatureHistogram
        {
            get
            {
                return m_featureHistogram;
            }
        }
        #endregion

      

        public ObservableCollection<Tuple<string, double, double>> AverageFeaturesPerField
        {
            get
            {
                ObservableCollection<Tuple<string, double, double>> result = new ObservableCollection<Tuple<string, double, double>>();

                AppDataCenter.Singleton.ChosenFields.Select((b, i) => new { field = b, index = i }).ToList().ForEach(a =>
                {
                    result.Add(new Tuple<string, double, double>(a.field, AppDataCenter.Singleton.EntireModelStats.GetFieldGrade(a.field), AppDataCenter.Singleton.EntireModelStats.GetFieldSucess(a.field)));
                });

                return (new ObservableCollection<Tuple<string, double, double>>(result.OrderBy(a => a.Item3).ToList()));
            }
            set
            {
             //   OnChange(FeaturePerField, value, x => AppDataCenter.Singleton.FeaturePerField = x);
            }
        }


    


       






     




        public ObservableCollection<KeyValuePair<string, double>> FeatureGradeList
        {          
            get
            {
                ObservableCollection<KeyValuePair<string, double>> result = new ObservableCollection<KeyValuePair<string, double>>();
                AppDataCenter.Singleton.ChosenFeatures.Select((b, i) => new { feature = b, index = i }).ToList().ForEach(a =>
             {
                 result.Add(new KeyValuePair<string, double>(a.feature.Name, (double)AppDataCenter.Singleton.EntireModelStats.fieldList.SelectMany(c => c.Grades).Where(d => d.Key == a.index).Select(e => e.Value).Max()));
             });

                return result;
            }
            set
            {
               
            }
        }


        public ObservableCollection<KeyValuePair<string, double>> FieldsFeatureGradeList
        {
            get
            {
                if (SelectedFeature.Key == null) return new ObservableCollection<KeyValuePair<string, double>>();
                ObservableCollection<KeyValuePair<string, double>> result = new ObservableCollection<KeyValuePair<string, double>>();
                AppDataCenter.Singleton.ChosenFields.Select((b, i) => new { field = b, index = i }).ToList().ForEach(a =>
                {
                    result.Add(new KeyValuePair<string, double>(a.field, AppDataCenter.Singleton.EntireModelStats.GetFeatureFieldGrade(
                        AppDataCenter.Singleton.ChosenFeatures.FindIndex(x => x.Name == SelectedFeature.Key), a.index)));
                });

                return result;
            }
            set
            {
                OnPropertyChanged("FieldsFeatureGradeList");
            }
        }
     




    }
}
