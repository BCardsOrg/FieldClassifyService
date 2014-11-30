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
using Accord.Math;

using System.Windows.Media;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{

    public class AnalyseViewModel : SectionViewModel
	{
        public AnalyseViewModel()
		{
            
            RegisterProperty(new Action(() => ResetReport()), NotifyGroup.StartAnalyzer);
            RegisterProperty("MultiRunGraph2", NotifyGroup.Progress);
            RegisterProperty(new Action(() => UpdateReport()), NotifyGroup.StatisticData);
            RegisterProperty("TotalResult", NotifyGroup.StatisticData);
            RegisterProperty("ConsoleMessage", NotifyGroup.Progress);
            RegisterProperty("RejectedFieldsPrecent", NotifyGroup.StatisticData);
            RegisterProperty("MatchFieldsPrecent", NotifyGroup.StatisticData);
            RegisterProperty("FPFieldsPrecent", NotifyGroup.StatisticData);
            RegisterProperty("FeatureSelectvertical", NotifyGroup.Configuration);
            RegisterProperty("FeatureSelecthorizontal", NotifyGroup.Configuration);
            RegisterProperty("EntireGraphs", NotifyGroup.ScatterData);
            RegisterProperty(new Action(() => UpdateConfiguration()), NotifyGroup.Configuration);

       
           
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
        public override bool IsShowPagesView
        {
            get
            {
                return false;
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
                      //  if (m_multiRunGraph.PlotArea.PlotControl != null)
                      //  {
                      //      m_multiRunGraph.PlotControl.RefreshPlot();
                      //  }
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
           /* m_actualGraph = new LineSeries();
            m_actualGraph.Title = "";
            m_actualGraph.Points.Clear();
            m_qualityGraph.Points.Clear();
            m_parametersValue.Clear();
            ParemeterAxis.Reset();*/
        }
		private void UpdateReport()
		{
            var reportData = AppDataCenter.Singleton.BaseReportData;
            if (reportData != null)
            {
                NoOfPages = AppDataCenter.Singleton.SetupData.Pages.Count();
                NoOfTrainPages = AppDataCenter.Singleton.TrainPages.Count();
                NoOfTestPages = NoOfPages - NoOfTrainPages;

                Match = reportData.Match;
                Reject = reportData.Reject;
                FP = reportData.FP;
                MatchQualaty = reportData.Qualaty;


                ParameterValue = reportData.ParameterValue;
            }

			OnPropertyChanged("Duration");
            OnPropertyChanged("IsPassCriteria");
        }

        int _leftFeature = 0;

        public int LeftFeature
        {
            get { return _leftFeature; }
            set { _leftFeature = value;
          
            }          
        }


        int _rightFeature = 0;

        public int RightFeature
        {
            get { return _rightFeature; }
            set { _rightFeature = value;
           
            }
        }

         void GetScatteredData(string[] fieldList)
        {
             int[] selectedFeatures= AppDataCenter.Singleton.ChosenFeatures.Select((a,i)=>new {index = i,selected = a.IsSelected}).Where(b=>b.selected).Select(c=>c.index).ToArray();
             _EntireGraphs = new List<List<PlotModel>>();
             m_fieldsSelected.ForEach(a => a.rectColor = Colors.Transparent);
             for (int i = 0; i < selectedFeatures.Length; i++)
             {
                 List<PlotModel> NewPlotList = new List<PlotModel>();
                 for (int j = 0; j < selectedFeatures.Length; j++)
                 {
                     NewPlotList.Add(GetNewPlotModel(AppDataCenter.Singleton.GetScatterPoints(fieldList, selectedFeatures[i], selectedFeatures[j]), AppDataCenter.Singleton.ChosenFeatures.ElementAt(i).Name, AppDataCenter.Singleton.ChosenFeatures.ElementAt(j).Name));
                 }
                 _EntireGraphs.Add(NewPlotList);
             }
         
           OnPropertyChanged("EntireGraphs");
           OnPropertyChanged("FieldsSelected");
        }

        public void UpdateScatter()
         {
             GetScatteredData(FieldsSelected.Where(b=>b.IsSelected).Select(a => a.Name).ToArray());
         }

        public Dictionary<Tuple<int, int>, List<ScatterSeries>> EntireDataScattered { get; set; }

        public List<List<PlotModel>> _EntireGraphs { get; set; }

        public List<List<PlotModel>> EntireGraphs { 
            get{                            
                return _EntireGraphs;

            }
            set{}
        }


        private PlotModel GetNewPlotModel( List<ScatterSeries> seriesIn,string FeatureA,string FeatureB)
        {
           
         PlotModel   result = new PlotModel();
         result.PlotMargins = new OxyThickness(1, 1, 1, 1);
         result.Padding = new OxyThickness(0, 0, 0, 0);
         result.TitleFontSize = 0;
         result.SubtitleFontSize = 0;
         result.TitlePadding = 0;
         //   result.Title = "Marker types";
            var linearAxis1 = new LinearAxis();
            linearAxis1.Position = AxisPosition.Bottom;
            linearAxis1.Title = FeatureA;
            
            result.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis();
            result.Axes.Add(linearAxis2);
            linearAxis2.Title = FeatureB;
          

            int index = 0;
            seriesIn.ForEach(a =>
            {
                a.ColorAxis = new ColorAxis();
                a.MarkerFill = GetColorForPlotByFieldIndex(index);
                m_fieldsSelected.Where(b => b.IsSelected).ElementAt(index).rectColor = (GetColorForSeriesByFieldIndex(index));
                a.MarkerStroke = GetColorForPlotByFieldIndex(index++);
                
              
              // a.MarkerType = MarkerType.Circle;
               a.MarkerSize = 5  -  index;
               a.MarkerStrokeThickness = 0;
              //  a.XAxisKey = "xAxis";
                result.Series.Add(a);
            });


          
            return (result);
        }


       
      
     


        LineSeries m_actualGraph;
        LineSeries m_qualityGraph;
        LinearAxis m_xAxis;
        LinearAxis m_yAxis;
        LineAnnotation m_selectedReportData;
      

        PlotModel m_multiRunGraph;
        public PlotModel MultiRunGraph2
        {
            get
            {
              
                //UpdateMultiRunGraph();
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


        public MarkerType GetMarkerType(int index)
        {
            switch (index)
            {
                case 0: return MarkerType.Circle;
                case 1: return MarkerType.Cross;
                case 2: return MarkerType.Diamond;
                case 3: return MarkerType.Plus;
                case 4: return MarkerType.Square;
                case 5: return MarkerType.Star;
                case 6: return MarkerType.Triangle;
                default: return MarkerType.Circle;

            }
        }

        public OxyColor GetColorForPlotByFieldIndex(int index)
        {
            switch (index)
            {
                case 0: return OxyColors.Blue;
                case 1: return OxyColors.Green;
                case 2: return OxyColors.Yellow;
                case 3: return OxyColors.Red;
                case 4: return OxyColors.Orange;
                case 5: return OxyColors.Purple;
                case 6: return OxyColors.Cyan;
                case 7: return OxyColors.Gray;
                case 8: return OxyColors.Pink;
             
                
                default: return OxyColors.Black;

            }
        }

        public Color GetColorForSeriesByFieldIndex(int index)
        {
            switch (index)
            {
                case 0: return Colors.Blue;
                case 1: return Colors.Green;
                case 2: return Colors.Yellow;
                case 3: return Colors.Red;
                case 4: return Colors.Orange;
                case 5: return Colors.Purple;
                case 6: return Colors.Cyan;
                case 7: return Colors.Gray;
                case 8: return Colors.Pink;


                default: return Colors.Black;

            }
        }

        List<SetupFieldViewModel> m_fieldsSelected;
        public IEnumerable<SetupFieldViewModel> FieldsSelected
        {
            get
            {
                return m_fieldsSelected;
            }
        }

        void UpdateConfiguration()
        {
            m_fieldsSelected = AppDataCenter.Singleton.SetupData.Fields.Where(x => x.NumApear > 0 && x.IsSelected == true)
                                                                       .OrderByDescending(a => a.NumApear)
                                                                       .Select(x => new SetupFieldViewModel(x.Name))
                                                                       .ToList();
        }




        public ObservableCollection<FeatureSelectModel> FeatureSelectvertical
        {
            get
            {
                return (AppDataCenter.Singleton.FeatureSelectvertical);
            }
            set
            {
                OnChange(FeatureSelectvertical, value, x => AppDataCenter.Singleton.FeatureSelectvertical = x);
            }
        }

        public ObservableCollection<FeatureSelectModel> FeatureSelecthorizontal
        {
            get
            {
                return (AppDataCenter.Singleton.FeatureSelecthorizontal);
            }
            set
            {
                OnChange(FeatureSelecthorizontal, value, x => AppDataCenter.Singleton.FeatureSelecthorizontal = x);
            }
        }



      

	}
}
