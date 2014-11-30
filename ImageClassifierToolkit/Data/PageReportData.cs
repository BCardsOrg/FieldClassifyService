using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Statistic;
using TiS.Recognition.FieldClassifyService.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data
{
	public class PageReportData
	{
		public PageReportData()
		{
			ClassMatch = new Dictionary<string, double>();
		}

		public PageData Page { get; set; }
		
		public IDictionary<string, double> ClassMatch { get; private set; }

        public bool IsTrain
        {
            get
            {
                return AppDataCenter.Singleton.TrainPages.Contains(Page) == true;
            }
        }
	}
}
