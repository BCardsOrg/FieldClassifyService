using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;



namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    class TrainRunSetCommand : CommandBaseAsyncUI
    {
        TrainRunCommand m_trainRunCmd;

        System.Threading.AutoResetEvent m_oneRunFinish = new System.Threading.AutoResetEvent(false);

        List<ReportData> m_reportsData = new List<ReportData>();

        Dictionary<string, double> m_parameters = new Dictionary<string, double>();

        public TrainRunSetCommand() :
            base(Shell.ShellDispatcher)
        {
            m_trainRunCmd = new TrainRunCommand();
            m_trainRunCmd.Executed += m_trainRunCmd_Executed;
        }

        void m_trainRunCmd_Executed(object sender, EventArgs e)
        {
            m_oneRunFinish.Set();
        }

     /*   public override void Execute(object state)
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
                    m_trainRunCmd.Configuration = AppDataCenter.Singleton.SetupData.ClassifierConfigurations.ElementAt(i);
                    m_trainRunCmd.ReportData = m_reportsData[i];
                    m_trainRunCmd.Execute(null);
                    m_oneRunFinish.WaitOne();
                }

                if (AppDataCenter.Singleton.ReportsData.Count() > 2)
                {
                    var result = AppDataCenter.Singleton.ReportsData.OrderBy(x => x, new AppDataCenter.CompareReportData()).First();

                    AppDataCenter.Singleton.SetReportByParameter(result.ParameterValue);
                }
            }
            finally
            {
                AppDataCenter.Singleton.FinishAnalyzer();
            }
        }*/

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
                if (parametersLeft.Count() > 1)
                {
                    BuildConfigurations(parametersLeft.Where(x => x != parameter).ToList());
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
                    if ( AppDataCenter.Singleton.IsAbortAnalyzer == true )
                    {
                        break;
                    }
                    m_trainRunCmd.Configuration = AppDataCenter.Singleton.SetupData.ClassifierConfigurations.ElementAt(i);
                    m_trainRunCmd.ReportData = m_reportsData[i];
                    m_trainRunCmd.Execute(null);
                    m_oneRunFinish.WaitOne();
                }

                if (AppDataCenter.Singleton.ReportsData.Count() > 2)
                {
                    var result = AppDataCenter.Singleton.ReportsData.OrderBy(x => x, new AppDataCenter.CompareReportData()).First();

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
