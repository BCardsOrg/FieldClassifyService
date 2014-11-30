
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Dialogs;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Statistic;
using TiS.Recognition.FieldClassifyService.Models;
using TiS.Recognition.FieldClassifyService.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    public class SaveModelDataCommand : CommandBaseAsyncUI
    {
        public SaveModelDataCommand() 
			: base(Shell.ShellDispatcher)
		{}

        protected override void LoadData(object state)
        {
        }

        protected override void UpdateView(object state)
        {

            var dlg = new SaveModelDataDlg();

            if ( dlg.ShowDialog() == true )
            {
                SetupData.Save(
                    AppDataCenter.Singleton.SetupData,
                    AppDataCenter.Singleton.GetSetupDataFileName(AppDataCenter.Singleton.InputFolder));

                ModelData modelData = new ModelData();
                
                modelData.Name = dlg.ModelName;
                modelData.CreateDate = DateTime.Now;
                modelData.TrainPages = new List<PageSetupData>(AppDataCenter.Singleton.TrainPages.Select( x => x.Setup ));
                modelData.AcceptanceCriteria = AppDataCenter.Singleton.AcceptanceCriteria;


                if (AppDataCenter.Singleton.ReportsData.Count() > 0)
                {
                    modelData.Match = AppDataCenter.Singleton.BaseReportData.Match;
                    modelData.Reject = AppDataCenter.Singleton.BaseReportData.Reject;
                    modelData.FP = AppDataCenter.Singleton.BaseReportData.FP;
                }
                else
                {
                    modelData.Match = 0;
                    modelData.Reject = 0;
                    modelData.FP = 0;
                }

                modelData.FeaturesSelectedNames = AppDataCenter.Singleton.FeaturesSelected
                                                                .Where(x => x.IsSelected)
                                                                .Select(x => x.Name)
                                                                .ToList();

              /*  modelData.FieldsSelectedNames = AppDataCenter.Singleton.SetupData.Fields
                                                                .Where(x => x.IsSelected == true)
                                                                .Select(x => x.Name)
                                                                .ToList();*/

                modelData.FieldsSelectedNames = AppDataCenter.Singleton.ChosenFields.ToList();

                modelData.ClassifierModel = new MemoryStream();

               

                ClassifierService.Service.SaveTrainData(
                       modelData.ClassifierModel, 
                       AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration, 
                       AppDataCenter.Singleton.InputFolder, 
                       AppDataCenter.Singleton.BaseReportData.PageThreshold);

                ModelsService.Service.Save(modelData);
            }

        }

        protected override void UpdateData(object state)
        {
        }
    }
}
