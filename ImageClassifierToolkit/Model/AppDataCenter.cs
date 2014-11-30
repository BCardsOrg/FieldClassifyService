using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using System.IO;
using System.Windows.Media.Imaging;
using TiS.Recognition.FieldClassifyService.Data;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using System.Collections.ObjectModel;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection;
using TiS.Recognition.FieldClassifyService.Service;
using TiS.Recognition.FieldClassifyService.Models;
using TiS.Recognition.FieldClassifyService.API_GoldenData;

using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.FromCore;
using OxyPlot;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model
{
	/// <summary>
	/// Notification group
	/// </summary>
	public class NotifyGroup
	{
        public const string StartAnalyzer = "StartAnalyzer";
        public const string HistorySaveData = "HistorySaveData";
		public const string StatisticData = "StatisticData";
		public const string Configuration = "Configuration"; 
		public const string Progress = "Progress";
        public const string Console = "Console";
        public const string ScatterData = "ScatterData";
        
	}

	public class AppDataCenter : SingletonViewModel<AppDataCenter>
	{
        public class CompareReportData : IComparer<ReportData>
        {
            public int Compare(ReportData x, ReportData y)
            {
                return AppDataCenter.Singleton.CompareBestResult(x, y);
            }
        }

        bool m_isRunning = false;


        public AppDataCenter()
        {
            AcceptanceCriteria = new AcceptanceCriteriaData() { ModelMaxFpPages = 0.2, ModelMinMatchPages = 0.5 };
            m_setupData = new SetupData();
            m_setupData.BaseClassifierConfiguration = ClassifierService.GetDefaultConfiguration();

            ModelsService.Service.ModelsFolder = CommandLine.GetParamValue("ModelsFolder");

            if (CommandLine.GetParamValue("FilterFields") != null)
            {
                _filterFields = CommandLine.GetParamValue("FilterFields").Split(',');
            }
            else
            {
                _filterFields = new string[0];
            }

            _pathtoscale = CommandLine.GetParamValue("scales");
          /*  if (TiS.Core.TisCommon.CommandLine.GetParamValue("scales") != null)
            {
                FeatureListHolder.loadScalesFromFile(TiS.Core.TisCommon.CommandLine.GetParamValue("scales"));
              
                FeatureListHolder.SaveScalesToFile(TiS.Core.TisCommon.CommandLine.GetParamValue("scales"));
            }*/
         


            ModelsService.Service.Changed += ModelsServiceService_Changed;

            FeaturesSelected = new ObservableCollection<IFieldFeature>();
            FeatureListHolder.GFeatures.ForEach(a => FeaturesSelected.Add(a));
            ReportResult = new ObservableCollection<InterfaceForReflection.Models.ReportResultItem>();
            ConsoleMessage = new ObservableCollection<string>();
            RejectedFields = new ObservableCollection<CustomFieldData>();

            FeatureSelectvertical = new ObservableCollection<FeatureSelectModel>();
            FeatureSelecthorizontal = new ObservableCollection<InterfaceForReflection.Models.FeatureSelectModel>();
            scatterList = new ObservableCollection<ScatterSeries>();
            ChosenFields = new string [0];
            ChosenFeatures = new List<IFieldFeature>();
            EntireModelStats = new InterfaceForReflection.Models.FeatureStatistics();

            FeatureGrades = new ObservableCollection<Tuple<string, double,double>>();

            var cmd = CommandLine.GetParamValue("FeatureName");
            if (cmd != null)
            {
                m_featuresCalculate = CommandLine.GetParamValue("FeatureName")
                                                                                      .Split(new char[] { ':' })
                                                                                      .ToList();
        }
        }


      public  void UpdateFeatureVertHorzSelection()
        {
            int index = 0;
            foreach (IFieldFeature field in FeaturesSelected.Where(a=>a.IsSelected))
            {
                FeatureSelectModel featureselect = new InterfaceForReflection.Models.FeatureSelectModel();
                featureselect.isChecked = false;
                featureselect.Name = field.Name;
                featureselect.SerialID = index++;
                FeatureSelectvertical.Add(featureselect);
                FeatureSelecthorizontal.Add(featureselect);
            }
            NotifyChange(NotifyGroup.Configuration);  
        }
      

        void RejectedFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyChange(NotifyGroup.StatisticData);           
        }



        void ModelsServiceService_Changed(object sender, EventArgs e)
        {
            NotifyChange(NotifyGroup.Configuration);
        }

        public void NotifyChange( string notifyGroup )
        {
            OnPropertyChanged(notifyGroup);
        }

        string[] _filterFields;
        public string[] FilterFields { get { return _filterFields; } }

        string _pathtoscale = null;
        public string PathToScale { get { return _pathtoscale; } }

		string m_message = "";
		public string Message
		{
			get
			{
				return m_message;
			}
			set
			{
				OnChange(ref m_message, value, "Message");
			}
		}

        //ImageClassifierConfiguration m_classifierConfiguration = ClassifierService.GetDefaultConfiguration();
        //public ImageClassifierConfiguration ClassifierConfiguration
        //{
        //    get
        //    {
        //        return m_classifierConfiguration;
        //    }
        //    set
        //    {
        //        m_classifierConfiguration = value;
        //        OnPropertyChanged(NotifyGroup.Configuration);
        //    }
        //}

        public string GetSetupDataFileName( string inputFolder )
        {
            return System.IO.Path.Combine(inputFolder ?? "", "SetupPages.bin");
        }

        SetupData m_setupData;
        public SetupData SetupData
        {
            get
            {
                return m_setupData;
            }
        }

      

        void m_currentSetupData_Changed(object sender, EventArgs e)
        {
            OnPropertyChanged(NotifyGroup.Configuration);
        }

        List<ReportData> m_reportsData = new List<ReportData>();

        public IEnumerable<ReportData> ReportsData
        {
            get
            {
                return m_reportsData;
            }
        }

        public bool IsRunning
        {
            get
            {
                return m_isRunning;
            }
        }

        private void ClearAnalyzerData()
        {
            m_reportsData.Clear();

            BaseReportData = null;

            this.SetupData.ClearConfigurationsList();

        }

		public bool StartAnalyzer()
		{
			if (m_isRunning == true)
			{
				//throw new Exception("Try to start Analyzer while Analyzer is already running!");
				System.Windows.MessageBox.Show("Try to start while it is already running!");
				return false;
			}

            IsAbortAnalyzer = false;

			m_isRunning = true;

            ClearAnalyzerData();

            base.OnPropertyChanged(NotifyGroup.StartAnalyzer);
            base.OnPropertyChanged(NotifyGroup.StatisticData);
			base.OnPropertyChanged(NotifyGroup.Progress);

			return true;
		}

        public bool IsAbortAnalyzer { get; private set; }
        internal void StopAnalyzer()
        {
            if (m_isRunning == true)
            {
                IsAbortAnalyzer = true;
            }
        }
        public void FinishAnalyzer()
		{
			m_isRunning = false;

            base.OnPropertyChanged(NotifyGroup.Progress);
			base.OnPropertyChanged(NotifyGroup.StatisticData);
		}



        internal void AddClassifierConfiguration(ConfigurationFieldClassifier config)
        {
            this.SetupData.AddClassifierConfiguration(config);
        }

        internal void AddReportData(ReportData reportData)
        {
            m_reportsData.Add(reportData);

            // Use the last report data to be the current report data
            BaseReportData = reportData;

            base.OnPropertyChanged(NotifyGroup.Progress);
            base.OnPropertyChanged(NotifyGroup.StatisticData);
        }

        public string InputFolder { get; private set; }

        public void BuildSetupData(string inputFolder, string setupDataFileName)
        {
            ClearAnalyzerData();

            m_trainPages.Clear();

            SetupData.Clear();

            if (string.IsNullOrEmpty(inputFolder) == false)
            {
                InputFolder = Path.GetFullPath(inputFolder);

                AddFolder(InputFolder);
                foreach (var dir in Directory.GetDirectories(InputFolder, "*.*", System.IO.SearchOption.AllDirectories))
                {
                    AddFolder(dir);
                }


                //SetupData.LoadGoldlData(setupDataFileName);
            }
            else
            {
                InputFolder = "";
            }

            OnPropertyChanged(NotifyGroup.Configuration);
        }

        private void AddFolder(string folderName, string className = "")
        {
            ObservableCollection<DocumentData> tempList = new ObservableCollection<DocumentData>();
            tempList.CollectionChanged += tempList_CollectionChanged;
            DocumentsLoader.GetAllDocuemntsNew(folderName, tempList);
            foreach (var doc in tempList)
            {
                SetupData.AddPage(new PageData() { Setup = new PageSetupData() { FileName = doc.ImageSource, PageNo = 1 }, DocData = doc });    
            }

            SetupData.RemoveBadFields(_filterFields);
              
           AddConsoleMessage("Done loading Documents"); 
        }

        void tempList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AddConsoleMessage(string.Format("Load page {0}", e.NewStartingIndex));
        }

     

        List<PageData> m_trainPages = new List<PageData>();
        public IEnumerable<PageData> TrainPages
        {
            get
            {
                return m_trainPages;
            }
        }

        public void ToggleTrainPages( IEnumerable<PageData> pages)
        {
            foreach (var page in pages)
            {
                if ( m_trainPages.Contains( page ) == true )
                {
                    m_trainPages.Remove(page);
                }
                else
                {
                    m_trainPages.Add(page);
                }
            }
            OnPropertyChanged(NotifyGroup.Configuration);
        }

        public void SelectTrainPages(double ratio, bool mixClasses)
        {
            m_trainPages.Clear();
            m_trainPages.AddRange(CategorizeService.SplitPerClass(SetupData.Pages, ratio, 1, mixClasses));
            OnPropertyChanged(NotifyGroup.Configuration);
        }

        public void SelectTrainPages(Func<PageData, int, bool> isTrainPage)
        {
            m_trainPages.Clear();
            foreach (var groupPages in SetupData.Pages.GroupBy(x => x.Setup.ClassName))
            {
                m_trainPages.AddRange(groupPages.Where((p, i) => isTrainPage(p, i) == true));
            }
            OnPropertyChanged(NotifyGroup.Configuration);
        }

        public void SelectTrainPages(IEnumerable<PageSetupData> trainPages)
        {
            if (trainPages == null || trainPages.Count() == 0)
            {
                m_trainPages.Clear();
                OnPropertyChanged(NotifyGroup.Configuration);
            }
            else
            {
                SelectTrainPages((p, i) =>
                {
                    return trainPages.Any(x => string.Compare(x.FileName, p.Setup.FileName, true) == 0 && x.PageNo == p.Setup.PageNo);
                });
            }
        }

        public bool SelectTrainPage(PageData page)
        {
            bool isAdd = false;

            var orgPage = SetupData.Pages.FirstOrDefault(x => string.Compare(x.Setup.FileName, page.Setup.FileName, true) == 0 && x.Setup.PageNo == page.Setup.PageNo);
            if (orgPage != null)
            {
                if (m_trainPages.Contains(orgPage) == false)
                {
                    m_trainPages.Add(orgPage);
                    OnPropertyChanged(NotifyGroup.Configuration);
                    isAdd = true;
                }
            }

            return isAdd;
        }

        public bool UnSelectTrainPage(PageData page)
        {
            bool isDelete = false;

            var orgPage = m_trainPages.FirstOrDefault(x => string.Compare(x.Setup.FileName, page.Setup.FileName, true) == 0 && x.Setup.PageNo == page.Setup.PageNo);
            if (orgPage != null)
            {
                m_trainPages.Remove(orgPage);
                OnPropertyChanged(NotifyGroup.Configuration);
                isDelete = true;
            }

            return isDelete;
        }

        public void SetModelFolder( string modelsFolder )
        {
            ModelsService.Service.ModelsFolder = modelsFolder;
            OnPropertyChanged(NotifyGroup.Configuration);
        }

        public void LoadModel( string modelName )
        {
            var model = ModelsService.Service.Load(modelName);

            string trainPath;

            ClearAnalyzerData();

            foreach (var feature in FeaturesSelected)
            {
                feature.IsSelected = model.FeaturesSelectedNames.Contains(feature.Name);
            }
            
            SetupData.BaseClassifierConfiguration = ClassifierService.GetSetupData(model.ClassifierModel, out trainPath);

            m_setupData = SetupData.Load(GetSetupDataFileName(trainPath));
            if ( m_setupData != null )
            {
                InputFolder = Path.GetFullPath(trainPath);
            }
            else
            {
                BuildSetupData(trainPath, AppDataCenter.Singleton.GetSetupDataFileName(trainPath));
            }

        
        //if  ( true )
        //    {
        //        FieldsSelected.Clear();
        //        m_setupData = SetupData.Load(GetSetupDataFileName(trainPath));
        //        InputFolder = Path.GetFullPath(trainPath);
        //        UpdateFieldsSelected(m_setupData.Pages.Select(x => x.DocData));
        //    }
        //    else
        //    {
        //        SetInputFolder(trainPath, AppDataCenter.Singleton.GetSetupDataFileName(trainPath));
        //    }


            this.SelectTrainPages(model.TrainPages);

            AcceptanceCriteria = model.AcceptanceCriteria;

            ClassifierService.ApplayModel(model);

            OnPropertyChanged(NotifyGroup.Configuration);
            OnPropertyChanged(NotifyGroup.StatisticData);
        }

        public ReportData BaseReportData { get; private set; }

        internal int SetReportByParameter(string parameterValue)
        {
            var reportData = m_reportsData.First(x => x.ParameterValue == parameterValue);

            var reportDataIndex = m_reportsData.IndexOf(reportData);

            SetupData.BaseClassifierConfiguration = SetupData.ClassifierConfigurations.ElementAt(reportDataIndex);

            BaseReportData = reportData;

            OnPropertyChanged(NotifyGroup.Configuration);
            OnPropertyChanged(NotifyGroup.StatisticData);

            return reportDataIndex;
        }


        internal void UpdateProgress()
        {
            

            OnPropertyChanged(NotifyGroup.Progress);
            

        }


        public bool IsCongigurationValid
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.Pages.Count() > 0;
            }
        }

        public bool IsCategorizeValid
        {
            get
            {
                return IsCongigurationValid &&
                    // Check all pages have gold class
                    SetupData.Pages.Count() > 0 &&
                    SetupData.Pages.Any(x => string.IsNullOrEmpty(x.Setup.ClassName) == true) == false;
            }
        }

        public bool IsTrainTestValid
        {
            get
            {
                return 
                    AppDataCenter.Singleton.TrainPages.Count() >= 2;
            }
        }

        public bool IsRunValid
        {
            get
            {
                return IsTrainTestValid;
            }
        }

        public bool PassCriteria( ReportData reportData )
        {
            return 
                reportData.Match >= AcceptanceCriteria.ModelMinMatchPages &&
                reportData.FP <= AcceptanceCriteria.ModelMaxFpPages;
        }

        public int CompareBestResult( ReportData reportData1, ReportData reportData2)
        {
            if ( reportData1.Match > reportData2.Match &&
                PassCriteria(reportData1) == true)
            {
                return -1;
            }
            else if (reportData2.Match > reportData1.Match &&
                PassCriteria(reportData2) == true)
            {
                return 1;
            }
            if (reportData1.Match == reportData2.Match)
            {
                if (reportData1.Qualaty > reportData2.Qualaty &&
                    PassCriteria(reportData1) == true)
                {
                    return -1;
                }
                else if (reportData2.Qualaty > reportData1.Qualaty &&
                    PassCriteria(reportData2) == true)
                {
                    return 1;
                }
            }

            return 0;
        }

      

        public AcceptanceCriteriaData AcceptanceCriteria { get; set; }
        public ObservableCollection<IFieldFeature> FeaturesSelected{get;set;}
        // The features we need to calculate each run - in case empty, all features will be calculate
        private List<string> m_featuresCalculate = new List<string>();
        public ObservableCollection<FeatureSelectModel> FeatureSelectvertical { get; set; }
        public ObservableCollection<FeatureSelectModel> FeatureSelecthorizontal { get; set; }
        
        public string[] ChosenFields { get; set; }

        public ObservableCollection<ReportResultItem> ReportResult { get; set; }

        public ObservableCollection<CustomFieldData> RejectedFields { get; set; }

       
        public ObservableCollection<ScatterSeries> scatterList { get; set; }
       
      
        public Double TotalResult {
            get
            {
                if (ReportResult == null) return 0;
                if (ReportResult.Count() == 0) return 0;
                return
                    Math.Round(
                    (double)ReportResult.SelectMany(a => a.ResultHist.Where((b) => b.Key == a.Name)).Select(c => c.Value)
                    .Aggregate((d, e) => d + e) / (double)ReportResult.SelectMany(a => a.ResultHist).Select(c => c.Value).Aggregate((d, e) => d + e) * 100
                    , 2);
            }

            set{   OnPropertyChanged(NotifyGroup.StatisticData);}
        }

        public void AddConsoleMessage(string message)
        {
            Dispatcher.BeginInvoke(
               (Action)delegate
               { ConsoleMessage.Add(message + Environment.NewLine); }
           );
        }

        public ObservableCollection<string> ConsoleMessage { get; set; }
       
        public FeatureStatistics EntireModelStats { get; set; }

        public List<ScatterSeries> GetScatterPoints(string[] FieldList,int feature1, int feature2)
        {
            if (EntireModelStats == null) return null;

            List<ScatterSeries> ScatterList = new List<ScatterSeries>();

           // scatterList.Clear();

         
            EntireModelStats.fieldList.Where(c=>FieldList.Contains(c.name)).ToList().ForEach(a =>
            {
                ScatterSeries seria = new ScatterSeries();
            
                a.Features.ForEach(b =>
                {
                    seria.Points.Add(new ScatterPoint(b[feature1], b[feature2]));

                });
               seria.Points =  seria.Points.Distinct().ToList();
                ScatterList.Add(seria);

            });

            return ScatterList;
          /*  ScatterSeries seriaNonGolden = new ScatterSeries();
            EntireModelStats.notAName.Features.ForEach(b =>
            {
                seriaNonGolden.Points.Add(new ScatterPoint(b[feature1], b[feature2]));

            });
            scatterList.Add(seriaNonGolden);*/
          
        }


        public ObservableCollection<Tuple<string, double,double>> FeatureGrades { get; set; }


        public ObservableCollection<KeyValuePair<string, double>> FeatureField { get; set; }

        public List<IFieldFeature> ChosenFeatures { get; set;}

        internal bool IsFeatureCalculate(string Name)
        {
            if (m_featuresCalculate.Count() > 0)
            {
                return m_featuresCalculate.Contains(Name);
            }
            else
            {
                return true;
            }
        }

        public bool IsAllFeaturesCalculate
        {
            get
            {
                // Check that we have already all features calculate, so we can used them & process only part of the features
                if (AppDataCenter.Singleton.SetupData.Pages.Any(x => x.DocData.Candidates != null) == true)
                {
                    return m_featuresCalculate.Count() == 0 || m_featuresCalculate.Count() == FeaturesSelected.Count;
                }
                else
                {
                    return true;
                }
            }
        }

        internal void AddFeatureCalculate(string name)
        {
            if (IsFeatureCalculate(name) == false)
            {
                m_featuresCalculate.Add(name);
            }
        }
        internal void RemoveFeatureCalculate(string name)
        {
            if (m_featuresCalculate.Count == 0)
            {
                m_featuresCalculate = AppDataCenter.Singleton.FeaturesSelected.Select(x => x.Name).ToList();
            }
            if (IsFeatureCalculate(name) == true)
            {
                m_featuresCalculate.Remove(name);
            }
        }

        // Grpou all pages by field content
        internal void SetPagesClass(string classifierField)
        {
            CategorizeService.SetPageClass(SetupData.Pages, classifierField);
            OnPropertyChanged(NotifyGroup.Configuration);
        }

        // Grpou all pages by file folder
        internal void SetPageCalssPerFolder()
        {
            foreach (var page in AppDataCenter.Singleton.SetupData.Pages)
            {
                var dir = Path.GetDirectoryName(page.Setup.FileName);
                page.Setup.ClassName = Path.GetFullPath(dir).Substring(InputFolder.Length).Trim(new[] { '\\' });
            }

            OnPropertyChanged(NotifyGroup.Configuration);
        }

        // Set pages class
        internal void SetGoldClass(IEnumerable<PageData> selectedPages, string goldClassName)
        {
            foreach (var page in selectedPages)
            {
                page.Setup.ClassName = goldClassName;
            }
            OnPropertyChanged(NotifyGroup.Configuration);
        }

        internal void UpdateFeatures( Action<IFieldFeature> updateFeature )
        {
            foreach (var featre in FeaturesSelected)
	        {
                updateFeature(featre);
	        } 
            OnPropertyChanged(NotifyGroup.Configuration);
        }

        internal void UpdateFields(Action<SetupFieldData> updateFeature)
        {
            foreach (var featre in SetupData.Fields)
            {
                updateFeature(featre);
            }
            OnPropertyChanged(NotifyGroup.Configuration);
        }
    }
}




