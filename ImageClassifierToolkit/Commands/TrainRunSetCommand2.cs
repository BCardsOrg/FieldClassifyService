using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.Classifier.ImageClassifierToolkit.Model;
using TiS.Recognition.Classifier.ImageClassifierToolkit.Data;
using TiS.Recognition.Classifier.ImageClassifierToolkit.Services;
using System.Threading.Tasks;
using TiS.Recognition.Classifier.ImageClassifierToolkit.Dialogs;
using TiS.Recognition.Classifier.ImageClassifierToolkit.ViewModels;
using System.Collections.Concurrent;
using System.Windows.Media.Imaging;
using TiS.Recognition.ImageClassifier.Service.Models;
using TiS.Recognition.ImageClassifier.Service;

namespace TiS.Recognition.Classifier.ImageClassifierToolkit.Commands
{
    public class TrainRunSetCommand2 : CommandBaseAsyncUI
    {
        List<ReportData> m_reportsData = new List<ReportData>();
        Dictionary<string, double> m_parameters = new Dictionary<string, double>();

        public TrainRunSetCommand2() :
            base(Shell.ShellDispatcher)
        { }

        public IEnumerable<ParameterRange> Parameters { get; private set; }


        void BuildConfigurations(IEnumerable<ParameterRange> parametersLeft)
        {
            // Initialize parameters dictionary
            if (m_parameters.Count == 0)
            {
                foreach (var par in parametersLeft)
                {
                    m_parameters.Add(par.ParameterName, 0);
                }
            }

            var parameter = parametersLeft.First();

            var fromValue = double.Parse(parameter.From);
            var toValue = double.Parse(parameter.To);
            var interval = double.Parse(parameter.Interval);

            // Create configuration & ReportData for each run
            for (var val = fromValue; val <= toValue; val += interval)
            {
                m_parameters[parameter.ParameterName] = val;
                if ( parametersLeft.Count() > 1 )
                {
                    BuildConfigurations( parametersLeft.Where(x => x != parameter).ToList());
                }
                else
                {
                    string parameterValue;

                    var config = ClassifierService.GenerateConfiguration(
                        m_parameters,
                        AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration, out parameterValue);

                    AppDataCenter.Singleton.AddClassifierConfiguration(config);

                    m_reportsData.Add(new ReportData() { ParameterValue = parameterValue, ParameterName = m_parameters.Count > 1 ? "Multi" : parameter.ParameterName });
                }
            }
        }

        protected override void LoadData(object state)
        {
            m_reportsData.Clear();
            m_parameters.Clear();

            AppDataCenter.Singleton.StartAnalyzer();

            try
            {
                var vm = state as MultiRunConfigurationViewModel;

                // Build set of configuration according the selected parameters
                BuildConfigurations(vm.Parameters.Where(x => x.IsSelected == true).ToList());

                int noOfRuns = AppDataCenter.Singleton.SetupData.ClassifierConfigurations.Count();


                for (int i = 0; i < noOfRuns; i++)
                {
                    ReportData reportData = m_reportsData[i];

                    var startTime = DateTime.Now;

                    // Train...


                    try
                    {
                        ConcurrentBag<Page> trainPages = new ConcurrentBag<Page>();
                        Parallel.ForEach(AppDataCenter.Singleton.TrainPages.GroupBy(x => x.FileName), pagesDate =>
                        {
                            var dec = TiffBitmapDecoder.Create(new Uri(pagesDate.Key), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            foreach (var pageData in pagesDate)
                            {
                                var frame = dec.Frames[pageData.PageNo - 1];
                                var image = ImageHelper.BitmapFromSource(frame);
                                var page = new Page()
                                {
                                    Image = image,
                                    ClassName = pageData.ClassName
                                };
                                trainPages.Add(page);

                                PageReportData pageReportData = new PageReportData() { Page = pageData };
                                pageReportData.ClassMatch.Clear();
                                pageReportData.ClassMatch.Add(page.ClassName, 1);
                                reportData.PagesReportData.Add(pageReportData);
                            }
                        });

                        ClassifierService.Service.Train(trainPages,
                                                        AppDataCenter.Singleton.SetupData.ClassifierConfigurations.ElementAt(i));

                        // Run...
                        Parallel.ForEach(AppDataCenter.Singleton.SetupData.Pages
                            .Where(x => AppDataCenter.Singleton.TrainPages.Contains(x) == false)
                            .GroupBy(x => x.FileName), pagesDate =>
                            {
                                var dec = TiffBitmapDecoder.Create(new Uri(pagesDate.Key), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                                foreach (var pageData in pagesDate)
                                {
                                    var frame = dec.Frames[pageData.PageNo - 1];
                                    var image = ImageHelper.BitmapFromSource(frame);
                                    var page = new Page()
                                    {
                                        Image = image,
                                        ClassName = ""
                                    };
                                    ClassifierService.Service.Run(page, null, classesMatch =>
                                    {
                                        PageReportData pageReportData = new PageReportData() { Page = pageData };

                                        if (classesMatch.Count() > 0)
                                        {
                                            foreach (var classMatch in classesMatch)
                                            {
                                                pageReportData.ClassMatch.Add(classMatch.Key, classMatch.Value);
                                            }
                                        }
                                        else
                                        {
                                            pageReportData.ClassMatch.Clear();
                                        }
                                        reportData.PagesReportData.Add(pageReportData);
                                    });


                                }
                            });

                    }
                    // In case the configuration is not valid
                    catch (Exception)
                    {
                    }

                    reportData.Duration = DateTime.Now - startTime;

                    // Add report data to system
                    AppDataCenter.Singleton.AddReportData(reportData);
                }

                if (AppDataCenter.Singleton.ReportsData.Count() > 2)
                {
                    var result = AppDataCenter.Singleton.ReportsData.OrderBy(x => x, new AppDataCenter.CompareReportData() ).First();

                    AppDataCenter.Singleton.SetReportByParameter(result.ParameterValue);
                }
            }
            finally
            {
                AppDataCenter.Singleton.FinishAnalyzer();
            }
        }

        protected override void UpdateView(object state)
        {
        }

        protected override void UpdateData(object state)
        {
        }
    }
}
