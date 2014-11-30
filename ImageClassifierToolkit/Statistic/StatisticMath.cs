using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Statistic
{
	static public class StatisticMath
	{
        static public MatchType CalcPageMatchType(PageReportData pageReport)
        {
            if (pageReport.ClassMatch.Count > 0)
            {
                var matchClass = pageReport.ClassMatch.First();
                if (matchClass.Key == pageReport.Page.Setup.ClassName)
                {
                    return MatchType.Match;
                }
                else
                {
                    return MatchType.FalsePositive;
                }
            }
            else
            {
                return MatchType.Reject;
            }
        }


        internal static void CalcPageMatchType(IEnumerable<PageReportData> pages, out double match, out double reject, out double fp, out double matchQualaty)
        {
            int matchCount = 0;
            int goodMatch = 0;
            int rejectCount = 0;
            int fpCount = 0;


            int noOfPages = pages.Count();

            if (noOfPages == 0)
            {
                match = 0;
                reject = 0;
                fp = 0;
                matchQualaty = 0;
                return;
            }

            List<double> pagesMatch = new List<double>();

            foreach (var page in pages)
            {
                switch (CalcPageMatchType(page))
                {
                    case MatchType.Match:
                        matchCount++;
                        double dis;
                        if (page.ClassMatch.Count > 1)
                        {
                            dis = page.ClassMatch.ElementAt(0).Value - page.ClassMatch.ElementAt(1).Value;
                        }
                        else
                        {
                            dis = page.ClassMatch.ElementAt(0).Value;
                        }
                        if (dis > 0.2)
                        {
                            goodMatch++;
                        }
                        pagesMatch.Add(page.ClassMatch.ElementAt(0).Value);
                        break;

                    case MatchType.Reject:
                        rejectCount++;
                        break;

                    case MatchType.FalsePositive:
                        fpCount++;
                        break;
                }
            }

            // Clac match
            match = (double)matchCount / noOfPages;

            // Clac match
            reject = (double)rejectCount / noOfPages;

            // Clac match
            fp = (double)fpCount / noOfPages;

            // Calc matchQualaty...
            if (pagesMatch.Count > 0)
            {
                // (Use Standard deviation)
                matchQualaty = Math.Sqrt(pagesMatch.Aggregate((s, x) => s + x * x) / pagesMatch.Count);
            }
            else
            {
                matchQualaty = 0;
            }
        }
	}
}
