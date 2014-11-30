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

    public class SummaryViewModel : SectionViewModel
    {
        public SummaryViewModel()
        {

          /*  RegisterProperty("FeatureGrades", NotifyGroup.StatisticData);
            RegisterProperty("FieldFeatureGrades", NotifyGroup.StatisticData);
            RegisterProperty("FalsePositiveResults", NotifyGroup.StatisticData);
       
            RegisterProperty("FPAndExpectedGrade", NotifyGroup.StatisticData);
            RegisterProperty("ChosenFieldReport", NotifyGroup.StatisticData);
            RegisterProperty("ChosenResult", NotifyGroup.StatisticData);

            RegisterProperty("NewConfidanceResult", NotifyGroup.StatisticData);*/
            RegisterProperty("FPMatchPlot", NotifyGroup.ScatterData);
                                       
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






        #region PlotMatchFP
        ColumnSeries m_featureHistogramPlotPoints;
        ColumnSeries m_featureHistogramPlotPointsOther;
        CategoryAxis m_xAxis;
        LinearAxis m_yAxis;
       


      
        public PlotModel FPMatchPlot
        {
            get
            {
                PlotModel m_FPMatchPlot = new PlotModel();


                m_FPMatchPlot.LegendSymbolLength = 24;
                m_FPMatchPlot.Title = "";
                var linearAxis1 = new LinearAxis();
                linearAxis1.Maximum = 100;
                linearAxis1.Minimum = 0;
                linearAxis1.Title = "%";
                m_FPMatchPlot.Axes.Add(linearAxis1);
                var linearAxis2 = new LinearAxis();
                linearAxis2.Maximum = 100;
                linearAxis2.Minimum = 0;
                linearAxis2.Position = AxisPosition.Bottom;
                linearAxis2.Title = "Confidnace Threshold";
                m_FPMatchPlot.Axes.Add(linearAxis2);

                var lineSeries1 = new LineSeries();
                lineSeries1.Color = OxyColors.SkyBlue;
                lineSeries1.MarkerFill = OxyColors.SkyBlue;
                lineSeries1.MarkerSize = 6;
                lineSeries1.MarkerStroke = OxyColors.White;
                lineSeries1.MarkerStrokeThickness = 1.5;
                lineSeries1.MarkerType = MarkerType.Circle;
                lineSeries1.Title = "Match";

                var lineSeries2 = new LineSeries();
                lineSeries2.Color = OxyColors.Teal;
                lineSeries2.MarkerFill = OxyColors.Teal;
                lineSeries2.MarkerSize = 6;
                lineSeries2.MarkerStroke = OxyColors.White;
                lineSeries2.MarkerStrokeThickness = 1.5;
                lineSeries2.MarkerType = MarkerType.Diamond;
                lineSeries2.Title = "False Positive";

                var lineSeries3 = new LineSeries();
                lineSeries3.Color = OxyColors.Orange;
                lineSeries3.MarkerFill = OxyColors.Orange;
                lineSeries3.MarkerSize = 6;
                lineSeries3.MarkerStroke = OxyColors.White;
                lineSeries3.MarkerStrokeThickness = 1.5;
                lineSeries3.MarkerType = MarkerType.Triangle;
                lineSeries3.Title = "Reject";

                for (double confidnace = 0; confidnace <= 100; confidnace++)
                {
                    double Match, FP;
                    AppDataCenter.Singleton.EntireModelStats.GetMatchFP(out Match, out FP, confidnace);

                    lineSeries1.Points.Add(new DataPoint(confidnace, Match));


                    lineSeries2.Points.Add(new DataPoint(confidnace, FP));
                    lineSeries3.Points.Add(new DataPoint(confidnace,100 - Match -  FP));

                }


                m_FPMatchPlot.Series.Add(lineSeries1);
                m_FPMatchPlot.Series.Add(lineSeries2);
              //  m_FPMatchPlot.Series.Add(lineSeries3);

                return m_FPMatchPlot;
            }
        }
        #endregion

      

       




    }
}
