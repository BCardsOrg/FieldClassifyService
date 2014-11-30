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
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls;
using TiS.Recognition.FieldClassifyService.API_GoldenData;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{

    public class AnalyseNumViewModel : SectionViewModel
    {
        public AnalyseNumViewModel()
        {
            RegisterProperty("AverageFeaturesPerField", NotifyGroup.StatisticData);
            RegisterProperty("FeatureGrades", NotifyGroup.StatisticData);
            RegisterProperty("FieldFeatureGrades", NotifyGroup.StatisticData);
            RegisterProperty("FalsePositiveResults", NotifyGroup.StatisticData);

            RegisterProperty("FPAndExpectedGrade", NotifyGroup.StatisticData);
            RegisterProperty("ChosenFieldReport", NotifyGroup.StatisticData);
            RegisterProperty("ChosenResult", NotifyGroup.StatisticData);


            RegisterProperty("NewConfidanceResult", NotifyGroup.StatisticData);


            viewmodeltub2 = new AnalyseNumViewModelTub2();
            Summary = new SummaryViewModel();



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

        public ObservableCollection<CandidateData> WordsCandidates { get; private set; }

        double m_imageZoom = 0.5;
        private void UpdateImageViewer()
        {
            // Reset WordsCandidates;
            if (WordsCandidates == null)
            {
                WordsCandidates = new ObservableCollection<CandidateData>();
            }
            WordsCandidates.Clear();

            // Save the last zoom
            if (m_imageViewerVM != null && m_imageViewerVM.ImageViewerViewModel != null)
            {
                m_imageZoom = m_imageViewerVM.ImageViewerViewModel.Zoom;
            }

            if (ChosenFieldReport == null)
            {
                if (m_imageViewerVM != null)
                {
                    m_imageViewerVM.ClearRois();
                }
                m_imageViewerVM = null;
            }
            else
            {
                int firstTiffPageIndex = ChosenFieldReport.Doc.PageNumber;
                int lastTiffPageIndex = firstTiffPageIndex;

                foreach (var field in ChosenFieldReport.Doc.Candidates)
                {
                    WordsCandidates.Add(field);
                }

                string tiffFileName = ChosenFieldReport.Doc.ImageSource;

                m_imageViewerVM = new ImageViewerMultiPageViewModel();
                m_imageViewerVM.FileName = tiffFileName;

                #region Set Colors
                m_imageViewerVM.Rois.FillColor = System.Windows.Media.Colors.Purple;
                m_imageViewerVM.Rois.BorderColor = System.Windows.Media.Colors.Purple;
                m_imageViewerVM.Rois.BorderThickness = 2;
                //Selected ROI format --> should not ever come into view, fallback only
                m_imageViewerVM.SelectedRois.FillColor = System.Windows.Media.Colors.Yellow;
                m_imageViewerVM.SelectedRois.BorderColor = System.Windows.Media.Colors.LightGreen;
                m_imageViewerVM.SelectedRois.BorderThickness = 2;
                //Selected ROI SubGroup1 format --> in Fields view used for recognition ROIs
                m_imageViewerVM.SelectedRoisSubGroup1.FillColor = System.Windows.Media.Color.FromArgb(0xFF, 0xFA, 0xBA, 0x6E); //was Yellow
                m_imageViewerVM.SelectedRoisSubGroup1.BorderColor = System.Windows.Media.Colors.Transparent;
                m_imageViewerVM.SelectedRoisSubGroup1.BorderThickness = 2;
                //Selected ROI SubGroup2 format--> in Fields view used for golden ROIs
                m_imageViewerVM.SelectedRoisSubGroup2.FillColor = System.Windows.Media.Colors.Transparent;
                m_imageViewerVM.SelectedRoisSubGroup2.BorderColor = System.Windows.Media.Color.FromRgb(0xF8, 0x79, 0x1C);
                m_imageViewerVM.SelectedRoisSubGroup2.BorderThickness = 2;
                //Recognition ROI format
                m_imageViewerVM.RecognitionRois.FillColor = System.Windows.Media.Color.FromArgb(0xFF, 0xFA, 0xBA, 0x6E);
                m_imageViewerVM.RecognitionRois.BorderColor = System.Windows.Media.Colors.Transparent;
                m_imageViewerVM.RecognitionRois.BorderThickness = 0;
                //Golden ROI format
                m_imageViewerVM.GoldenRois.FillColor = System.Windows.Media.Colors.Transparent;
                m_imageViewerVM.GoldenRois.BorderColor = System.Windows.Media.Color.FromRgb(0xF8, 0x79, 0x1C);
                m_imageViewerVM.GoldenRois.BorderThickness = 2;

                #endregion                    //}

                m_imageViewerVM.FirstPageIndex = firstTiffPageIndex;
                m_imageViewerVM.LastPageIndex = lastTiffPageIndex;


                if (!m_imageViewerVM.ImageViewerPages.ContainsKey(firstTiffPageIndex))
                {
                    m_imageViewerVM.ImageViewerPages.Add(firstTiffPageIndex, new ImageViewerPage());
                }

                // Add ROIs
                if (ChosenFieldReport.ExpectedField.IsEmpty == false)
                {
                    foreach (var ocrGoldWord in ChosenFieldReport.ExpectedField.Words)
                    {
                        var expectedFieldRoi = new Roi()
                        {
                            Rect = new System.Windows.Rect(
                                        ocrGoldWord.OriginRectangle.X,
                                        ocrGoldWord.OriginRectangle.Y,
                                        ocrGoldWord.OriginRectangle.Width,
                                        ocrGoldWord.OriginRectangle.Height),
                            LinkedObject = ocrGoldWord
                        };
                        m_imageViewerVM.ImageViewerPages[firstTiffPageIndex].GoldenRois.Add(expectedFieldRoi);
                    }
                }

                if (ChosenFieldReport.Field.IsEmpty == false)
                {
                    foreach (var ocrRecWord in ChosenFieldReport.Field.Words)
                    {
                        var recFieldRoi = new Roi()
                        {
                            Rect = new System.Windows.Rect(
                                        ocrRecWord.OriginRectangle.X,
                                        ocrRecWord.OriginRectangle.Y,
                                        ocrRecWord.OriginRectangle.Width,
                                        ocrRecWord.OriginRectangle.Height),
                            LinkedObject = ocrRecWord
                        };
                        m_imageViewerVM.ImageViewerPages[firstTiffPageIndex].RecognitionRois.Add(recFieldRoi);
                    }
                }

                m_imageViewerVM.SwitchToPage(firstTiffPageIndex);

                if (m_imageViewerVM.ImageViewerViewModel != null)
                {
                    m_imageViewerVM.ImageViewerViewModel.Zoom = m_imageZoom;
                }
            }

            OnPropertyChanged("ImageViewerVM");
            OnPropertyChanged("WordsCandidates");
        }

        internal void HighlightWordCandidate(IEnumerable<CandidateData> wordsCandidates)
        {
            // Clear prev candidate ROI
            var remove = m_imageViewerVM.ImageViewerViewModel.RecognitionRois.Where(x => x.LinkedObject == "WordCandidate").ToList();
            foreach (var item in remove)
            {
                m_imageViewerVM.ImageViewerViewModel.Rois.Remove(item);
            }

            m_imageViewerVM.ImageViewerViewModel.SelectedRois.Clear();

            // Add new candidate ROI
            foreach (var wordCandidate in wordsCandidates.SelectMany(x => x.Words))
            {
                var originalWordRec = wordCandidate.OriginRectangle;
                var recFieldRoi = new Roi()
                {
                    Rect = new System.Windows.Rect(
                            originalWordRec.X,
                            originalWordRec.Y,
                            originalWordRec.Width,
                            originalWordRec.Height),
                    LinkedObject = "WordCandidate"
                };

                m_imageViewerVM.ImageViewerViewModel.RecognitionRois.Add(recFieldRoi);
                m_imageViewerVM.ImageViewerViewModel.SelectedRois.Add(recFieldRoi);
            }
        }

        ImageViewerMultiPageViewModel m_imageViewerVM;
        public ImageViewerMultiPageViewModel ImageViewerVM
        {
            get
            {
                return m_imageViewerVM;
            }
        }


        #region tub1

        string m_chosenField;
        public string ChosenField
        {
            get
            {
                return m_chosenField;
            }
            set
            {
                if (OnChange(ref m_chosenField, value, "ChosenField") == true)
                {
                    SelectedFeature = default(KeyValuePair<string, double>);
                    UpdatePlot();
                }
            }
        }

        FieldReportItem m_chosenFieldReport;
        public FieldReportItem ChosenFieldReport
        {
            get
            {
                return m_chosenFieldReport;
            }
            set
            {
                if (OnChange(ref m_chosenFieldReport, value, "ChosenFieldReport") == true)
                {
                    UpdateImageViewer();
                }
            }
        }
        public Tuple<string, double, double, double, double, double> ChosenResult { get; set; }

        public double NewConfidanceResult { get; set; }



        public ObservableCollection<KeyValuePair<string, double>> FeatureGrades
        {
            get
            {
                ObservableCollection<KeyValuePair<string, double>> result = new ObservableCollection<KeyValuePair<string, double>>();
                AppDataCenter.Singleton.ChosenFeatures.Select((b, i) => new { feature = b, index = i }).ToList().ForEach(a =>
                {
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
                    UpdatePlot();

                }
            }
        }

        public ObservableCollection<KeyValuePair<string, double>> FieldFeatureGrades
        {
            get
            {
                if (ChosenField == null) return null;
                ObservableCollection<KeyValuePair<string, double>> result = new ObservableCollection<KeyValuePair<string, double>>();
                AppDataCenter.Singleton.ChosenFeatures.Select((b, i) => new { feature = b, index = i }).ToList().ForEach(a =>
                {
                    result.Add(new KeyValuePair<string, double>(a.feature.Name, AppDataCenter.Singleton.EntireModelStats.fieldList.Where(b => b.name == ChosenField).FirstOrDefault().Grades[a.index]));
                });
                return (new ObservableCollection<KeyValuePair<string, double>>(result.OrderBy(a => a.Value).ToList()));
            }
            set
            {
                //  OnChange(FeatureGrades, value, x => AppDataCenter.Singleton.FeatureGrades = x);
            }
        }


        #region fieldHistogram
        ColumnSeries m_featureHistogramPlotPoints;
        ColumnSeries m_featureHistogramPlotPointsOther;
        CategoryAxis m_xAxis;
        LinearAxis m_yAxis;
        private void UpdatePlot()
        {
            if (ChosenField == null) return;
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

            var field = AppDataCenter.Singleton.EntireModelStats.fieldList.First(b => b.name == ChosenField);
            var fields = AppDataCenter.Singleton.EntireModelStats.fieldList.Where(b => b.name != ChosenField).ToList();
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
                    m_featureHistogramPlotPointsOther.Items.Add(new ColumnItem(histogramData.Value, -1, OxyColors.Blue));

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


        public ObservableCollection<FieldReportItem> FalsePositiveResults
        {
            get
            {
                if (AppDataCenter.Singleton.EntireModelStats.testresults != null)
                {
                    return (new ObservableCollection<FieldReportItem>(AppDataCenter.Singleton.EntireModelStats.testresults.Where(a => (!a.IsMatch) && a.MatchedName == ChosenField)));
                }
                else
                {
                    return null;
                }
            }
            set
            {
                //   OnChange(FeaturePerField, value, x => AppDataCenter.Singleton.FeaturePerField = x);
            }
        }


        public IEnumerable<Tuple<string, double, double, double, double, double>> FPAndExpectedGrade
        {
            get
            {
                var result = new List<Tuple<string, double, double, double, double, double>>();

                if (ChosenFieldReport == null || ChosenField == null) return result;

                for (int i = 0; i < AppDataCenter.Singleton.ChosenFeatures.Count; i++)
                {
                    var feature = AppDataCenter.Singleton.ChosenFeatures[i];

                    double recFieldConfidence;
                    double goldFieldConfidence;

                    if (ChosenFieldReport.Field.IsEmpty == true)
                    {
                        recFieldConfidence = ChosenFieldReport.MatchQuality;
                    }
                    else
                    {
                        recFieldConfidence = ChosenFieldReport.Field.Features[i];
                    }
                    if (ChosenFieldReport.ExpectedField.IsEmpty == true)
                    {
                        goldFieldConfidence = ChosenFieldReport.ExpectedMatchQuality;
                    }
                    else
                    {
                        goldFieldConfidence = ChosenFieldReport.ExpectedField.Features[i];
                    }
                    result.Add(new Tuple<string, double, double, double, double, double>(feature.Name,
                          Math.Round(AppDataCenter.Singleton.EntireModelStats.fieldList.Where(b => b.name == ChosenField).FirstOrDefault().GetSTD(i), 1),
                        Math.Round(AppDataCenter.Singleton.EntireModelStats.fieldList.Where(b => b.name == ChosenField).FirstOrDefault().GetAverge(i), 1),
                        recFieldConfidence,
                        goldFieldConfidence
                        , Math.Abs(recFieldConfidence - goldFieldConfidence)

                        ));
                }
                return result;
            }
        }


        #endregion



        #region Tub2

        public AnalyseNumViewModelTub2 viewmodeltub2 { get; set; }



        #endregion

        #region Tub3
        public SummaryViewModel Summary { get; set; }
        #endregion

    }
}
