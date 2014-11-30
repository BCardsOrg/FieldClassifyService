using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    class SelectBestReportDataCommand : CommandBaseAsyncUI 
    {
        public SelectBestReportDataCommand() :
			base(Shell.ShellDispatcher)
		{}

        protected override void LoadData(object state)
        {
            if (AppDataCenter.Singleton.ReportsData.Count() > 2)
            {
                var result = AppDataCenter.Singleton.ReportsData.OrderBy(x => x, new AppDataCenter.CompareReportData()).First();

                AppDataCenter.Singleton.SetReportByParameter(result.ParameterValue);
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
