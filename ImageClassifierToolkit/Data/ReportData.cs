using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Statistic;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data
{
	public class ReportData
	{
        bool m_isValidStatistic = false;
        double m_matchQualaty;
        double m_match;
        double m_reject;
        double m_fp;

          public long matchCount { get; set; }
          public long rejectCount { get; set; }
          public long fpCount { get; set; }

          public long noOfFields { get; set; }
        public List<double> QualityMatch { get; set; }

		public ReportData()
		{
            PagesReportData = new System.Collections.Concurrent.ConcurrentBag<PageReportData>();
            PageThreshold = 0;
            Duration = TimeSpan.Zero;
            QualityMatch = new List<double>();
        }

        public System.Collections.Concurrent.ConcurrentBag<PageReportData> PagesReportData { get; private set; }

		public TimeSpan Duration { get; set; }

        public string ParameterValue { get; set; }

        public string ParameterName { get; set; }

        public double PageThreshold { get; set; }
        
        public void Clear()
		{
			Duration = TimeSpan.Zero;
            PagesReportData = new System.Collections.Concurrent.ConcurrentBag<PageReportData>();
		}

        private void ClacStatistic()
        {
            if (noOfFields == 0) return;

            m_match = (double)matchCount / noOfFields;

            // Clac match
            m_reject = (double)rejectCount / noOfFields;

            // Clac match
            m_fp = (double)fpCount / noOfFields;


            if (matchCount > 0 && QualityMatch.Count > 0)
            {
                // (Use Standard deviation)
                m_matchQualaty = Math.Sqrt(QualityMatch.Aggregate((s, x) => s + x * x) / matchCount);
            }
            else
            {
                m_matchQualaty = 0;
            }



          //  if (m_isValidStatistic == false)
          //  {
             //   StatisticMath.CalcPageMatchType(PagesReportData.Where(x => x.IsTrain == false), out m_match, out m_reject, out m_fp, out m_matchQualaty);

              //  PageThreshold = StatisticMath.FindPageThreshold(PagesReportData.Where(x => x.IsTrain == false), 
               //     AppDataCenter.Singleton.AcceptanceCriteria.ModelMinMatchPages,
               //     AppDataCenter.Singleton.AcceptanceCriteria.ModelMaxFpPages);

              //  m_isValidStatistic = true;
         //   }
        }

        public double Match
        {
            get
            {
               ClacStatistic();
                return m_match;
            }
        }

        public double Reject
        {
            get
            {
                ClacStatistic();
                return m_reject;
            }
        }

        public double FP
        {
            get
            {
                ClacStatistic();
                return m_fp;
            }
        }

        public double Qualaty
        {
            get
            {
                ClacStatistic();
                return m_matchQualaty;
            }
        }



        public int NoOfPages { get; set; }
    }
}
