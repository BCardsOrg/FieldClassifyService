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
using OxyPlot.Wpf;


using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls.OxyPlot;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{

    public class ReportViewModel : SectionViewModel
	{
		public ReportViewModel()
		{
            m_multiRunGraph = CreateMultiRunGraph();
            RegisterProperty(new Action(() => ResetReport()), NotifyGroup.StartAnalyzer);
            RegisterProperty("MultiRunGraph", NotifyGroup.Progress);
            RegisterProperty(new Action(() => UpdateReport()), NotifyGroup.StatisticData);
            RegisterProperty("TotalResult", NotifyGroup.StatisticData);
            RegisterProperty("ConsoleMessage", NotifyGroup.Progress);
            RegisterProperty("RejectedFieldsPrecent", NotifyGroup.StatisticData);
            RegisterProperty("MatchFieldsPrecent", NotifyGroup.StatisticData);
            RegisterProperty("FPFieldsPrecent", NotifyGroup.StatisticData);
                                    
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

		public TimeSpan Duration
		{
			get
			{
                if (AppDataCenter.Singleton.BaseReportData != null)
                {
                    return AppDataCenter.Singleton.BaseReportData.Duration;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }

		}

        int m_noOfPages = 0;
		public int NoOfPages
		{
			get
			{
				return m_noOfPages;
			}
			set
			{
				OnChange(ref m_noOfPages, value, "NoOfPages");
			}
		}

        int m_noOfTrainPages = 0;
        public int NoOfTrainPages
        {
            get
            {
                return m_noOfTrainPages;
            }
            set
            {
                OnChange(ref m_noOfTrainPages, value, "NoOfTrainPages");
            }
        }

        string m_parameterValue;
        public string ParameterValue
        {
            get
            {
                return m_parameterValue;
            }
            set
            {
                if ( OnChange(ref m_parameterValue, value, "ParameterValue") == true )
                {
                    if (m_parameterValue != null)
                    {
                        m_selectedReportData.X = AppDataCenter.Singleton.SetReportByParameter(m_parameterValue);
                      // if (m_multiRunGraph.PlotControl != null)
                       // {
                       //     m_multiRunGraph.PlotControl.RefreshPlot();
                       // }
                    }
                }
            }
        }

        ObservableCollection<string> m_parametersValue = new ObservableCollection<string>();

        public IEnumerable<string> ParametersValue
        {
            get
            {
                return m_parametersValue;
            }
        }

        int m_noOfTestPages = 0;
        public int NoOfTestPages
        {
            get
            {
                return m_noOfTestPages;
            }
            set
            {
                OnChange(ref m_noOfTestPages, value, "NoOfTestPages");
            }
        }

        double m_match;
		public double Match
		{
			get
			{
				return m_match;
			}
			set
			{
				OnChange(ref m_match, value, "Match");
			}
		}

		double m_reject;
		public double Reject
		{
			get
			{
				return m_reject;
			}
			set
			{
				OnChange(ref m_reject, value, "Reject");
			}
		}

		double m_fp;
		public double FP
		{
			get
			{
				return m_fp;
			}
			set
			{
				OnChange(ref m_fp, value, "FP");
			}
		}

		double m_matchQualaty;
		public double MatchQualaty
		{
			get
			{
				return m_matchQualaty;
			}
			set
			{
				OnChange(ref m_matchQualaty, value, "MatchQualaty");
			}
		}

        public bool IsPassCriteria
        {
            get
            {
                if (AppDataCenter.Singleton.BaseReportData != null)
                {
                    return AppDataCenter.Singleton.PassCriteria(AppDataCenter.Singleton.BaseReportData);
                }
                else
                {
                    return false;
                }
            }
        }

        public double TrainSize { get; set; }

        private void ResetReport()
        {
          //  m_actualGraph.Title = "";
          //  m_actualGraph.Points.Clear();
          //  m_qualityGraph.Points.Clear();
          //  m_parametersValue.Clear();
          //  ParemeterAxis.Reset();
        }
		private void UpdateReport()
		{
            var reportData = AppDataCenter.Singleton.BaseReportData;
            if (reportData != null)
            {
                NoOfPages = AppDataCenter.Singleton.SetupData.Pages.Count();
                NoOfTrainPages = AppDataCenter.Singleton.TrainPages.Count();
                NoOfTestPages = (int)reportData.NoOfPages;

                Match = reportData.Match;
                Reject = reportData.Reject;
                FP = reportData.FP;
                MatchQualaty = reportData.Qualaty;


                ParameterValue = reportData.ParameterValue;
            }

			OnPropertyChanged("Duration");
            OnPropertyChanged("IsPassCriteria");
        }




        private OxyPlot.PlotModel CreateMultiRunGraph()
        {
            return null;
           /* var graph = new OxyPlot.Wpf.Plot();
            graph.PlotAreaBorderColor = OxyColor.FromRgb(0x92, 0x92, 0x92);
            graph.PlotAreaBorderThickness = 1;
            graph. = false;

            graph.LegendFont = "Lato Light";
            graph.LegendTextColor = OxyColor.FromRgb(0x92, 0x92, 0x92);
            graph.LegendBorderThickness = 0;
            graph.LegendBackground = OxyColors.Transparent;
            graph.LegendOrientation = LegendOrientation.Horizontal;
            graph.LegendPlacement = LegendPlacement.Outside;

            m_selectedReportData = new OxyPlot.Wpf.LineAnnotation();
            m_selectedReportData.Type = OxyPlot.Wpf.LineAnnotationType.Vertical;
            m_selectedReportData.Color = OxyPlot.OxyColors.Green;
            m_selectedReportData.StrokeThickness = 2;

            graph.Annotations.Add(m_selectedReportData);

            m_xAxis = new ParemeterAxis(AxisPosition.Bottom)
            {
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0,
                Font = "Lato Light",
                TextColor = OxyColor.FromRgb(0x92, 0x92, 0x92),
                IsPanEnabled = false,
                IsZoomEnabled = false
            };
            graph.Axes.Add(m_xAxis);

            // Setup Y-Axis
            m_yAxis = new OxyPlot.Wpf.LinearAxis(OxyPlot.Wpf.AxisPosition.Left, 0d, 1d)
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineThickness = 0,
                MajorGridlineThickness = 1,
                //MajorStep = 100,
                TickStyle = TickStyle.None,
                StringFormat = "P0",
                Font = "Lato Light",
               // TextColor = OxyColor.FromRgb(0x92, 0x92, 0x92),
                IsPanEnabled = false,
                IsZoomEnabled = false

            };
            graph.Axes.Add(m_yAxis);


            m_actualGraph = new OxyPlot.Wpf.LineSeries();

            m_actualGraph.Color = OxyColors.Blue;

            m_actualGraph.Smooth = false;
            m_actualGraph.StrokeThickness = 2;

            graph.Series.Add(m_actualGraph);

            m_qualityGraph = new OxyPlot.Wpf.LineSeries();
            m_qualityGraph.Color = OxyColors.Green;
            m_qualityGraph.Smooth = false;
            m_qualityGraph.StrokeThickness = 2;
            m_qualityGraph.Title = "Quality";

            graph.Series.Add(m_qualityGraph);

            return graph;*/

        }


       OxyPlot.Wpf.LineSeries m_actualGraph;
       OxyPlot.Wpf.LineSeries m_qualityGraph;
       OxyPlot.Wpf.LinearAxis m_xAxis;
       OxyPlot.Wpf.LinearAxis m_yAxis;
       OxyPlot.Wpf.LineAnnotation m_selectedReportData;
        private void UpdateMultiRunGraph()
        {
            // It is not multi run, so we do not draw any graph
            if (AppDataCenter.Singleton.SetupData.ClassifierConfigurations.Count() == 0 )
            {
                return;
            }

            // Create the model
            lock (m_multiRunGraph)
            {
                if (m_multiRunGraph == null)
                {
                    m_multiRunGraph = CreateMultiRunGraph();
                }

                // Update the model with new result
                var noOfReports = AppDataCenter.Singleton.ReportsData.Count();
                while ( m_parametersValue.Count < noOfReports )
                {
                    var lastReportDataIndex = m_parametersValue.Count;
                    // Fill graph point for each ReportData
                  //  while (AppDataCenter.Singleton.SetupData.ClassifierConfigurations.Count() > m_actualGraph.Points.Count)
                  //  {
                      //  m_actualGraph.Points.Add(new DataPoint(m_actualGraph.Points.Count, 0.0));
                      //  m_qualityGraph.Points.Add(new DataPoint(m_qualityGraph.Points.Count, 0.0));
                  //  }

                    var reportData = AppDataCenter.Singleton.ReportsData.ElementAt(lastReportDataIndex);
                    m_parametersValue.Add(reportData.ParameterValue);
                    m_actualGraph.Title = reportData.ParameterName;
                    ParemeterAxis.AddTitle(lastReportDataIndex, reportData.ParameterValue);
                //    m_actualGraph.Points.ElementAt(lastReportDataIndex).Y = reportData.Match;
                 //   m_qualityGraph.Points.ElementAt(lastReportDataIndex).Y = AppDataCenter.Singleton.PassCriteria(reportData) ? reportData.Qualaty : 0d;
                }

              //  if (m_multiRunGraph.PlotControl != null)
               // {
               //     m_multiRunGraph.PlotControl.RefreshPlot();
               // }
                
            }
        }

        PlotModel m_multiRunGraph;
        public PlotModel MultiRunGraph
        {
            get
            {
                UpdateMultiRunGraph();
                return m_multiRunGraph;
            }
        }

        public ObservableCollection<ReportResultItem> ReportResult
        {
            get
            {
                return (AppDataCenter.Singleton.ReportResult);
            }
            set
            {
                OnChange(ReportResult, value, x => AppDataCenter.Singleton.ReportResult = x);
            }
        }

        public double TotalResult
        {
            get
            {
                return (AppDataCenter.Singleton.TotalResult);
            }
            set
            {
                OnChange(TotalResult, value, x => AppDataCenter.Singleton.TotalResult = x);
            }
        }

        public ObservableCollection<string> ConsoleMessage
        {
            get
            {
                return (AppDataCenter.Singleton.ConsoleMessage);
            }
            set
            {
                OnChange(ConsoleMessage, value, x => AppDataCenter.Singleton.ConsoleMessage = x);
            }
        }

        public double RejectedFieldsPrecent
        {
            get
            {
                ReportData repdat = AppDataCenter.Singleton.ReportsData.LastOrDefault();
                if(repdat == null) return 0;
                if (repdat.noOfFields == 0) return 0;
                return ((Math.Round((double)repdat.rejectCount / (double)repdat.noOfFields * 100, 2)));

              /*  double rejectedf;
                if (( rejectedf =  AppDataCenter.Singleton.RejectedFields.Count) == 0) return 0;
                if (( AppDataCenter.Singleton.ReportResult.Count) == 0) return 100;
                int otherf =  AppDataCenter.Singleton.ReportResult.SelectMany(a => a.ResultHist)
                    .ToList()
                    .Select(b => b.Value)
                    .ToList().Aggregate<int>((i, j) => i + j);

                if (otherf == 0) return 100;
                    

                return (
                    Math.Round((rejectedf  /       (double)  (otherf + rejectedf) * 100 ),2));*/
            }
            set {  }
        }

        public double MatchFieldsPrecent
        {
            get
            {
                ReportData repdat = AppDataCenter.Singleton.ReportsData.LastOrDefault();
                if (repdat == null) return 0;
                if (repdat.noOfFields == 0) return 0;
                return ((Math.Round((double)repdat.matchCount / (double)repdat.noOfFields * 100, 2)));

             
            }
            set { }
        }

        public double FPFieldsPrecent
        {
            get
            {
                ReportData repdat = AppDataCenter.Singleton.ReportsData.LastOrDefault();
                if (repdat == null) return 0;
                if (repdat.noOfFields == 0) return 0;
                return (Math.Round((double)repdat.fpCount / (double)repdat.noOfFields * 100, 2));


            }
            set { }
        }
        

        

        protected void OnChange<T>(T member, T newValue, Action<T> setMember)
        {
            if (member.Equals(newValue) == false)
            {
                setMember(newValue);
                AppDataCenter.Singleton.NotifyChange(NotifyGroup.StatisticData);
            }
        }




      

	}
}




