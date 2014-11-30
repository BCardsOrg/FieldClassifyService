using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.DataModel
{
    public class TisFormatting
    {
        public TisFormatting()
        {
        }

        public string ToTisDateString(DateTime oDateTime)
        {
            return String.Format(
                "{0,4:0000}{1,2:00}{2,2:00}",
                oDateTime.Year,
                oDateTime.Month,
                oDateTime.Day);
        }

        public string ToTisTimeString(DateTime oDateTime)
        {
            // TODO: Use system time format
            return String.Format(
                "{0,2:00}:{1,2:00}",
                oDateTime.Hour,
                oDateTime.Minute);
        }

        public DateTime TisDateToDateTime(string sTisDate)
        {
            if (sTisDate.Length != 8)
            {
                throw new TisException("Invalid TiS date format [{0}]", sTisDate);
            }

            string sYear = sTisDate.Substring(0, 4);
            string sMonth = sTisDate.Substring(4, 2);
            string sDay = sTisDate.Substring(6, 2);

            try
            {
                int nYear = int.Parse(sYear);
                int nMonth = int.Parse(sMonth);
                int nDay = int.Parse(sDay);

                return new DateTime(nYear, nMonth, nDay);
            }
            catch (FormatException oExc)
            {
                throw new TisException(oExc, "Failed to parse TiS date string [{0}]", sTisDate);
            }
        }

        public DateTime TisTimeToDateTime(string sTisTime)
        {
            if (sTisTime.Length != 5)
            {
                throw new TisException("Invalid TiS time format [{0}]", sTisTime);
            }

            string sHour = sTisTime.Substring(0, 2);
            string sMinute = sTisTime.Substring(3, 2);

            try
            {
                int nHour = int.Parse(sHour);
                int nMinute = int.Parse(sMinute);

                DateTime oNow = DateTime.Now;

                return new DateTime(oNow.Year, oNow.Month, oNow.Day, nHour, nMinute, 0);
            }
            catch (FormatException oExc)
            {
                throw new TisException(oExc, "Failed to parse TiS time string [{0}]", sTisTime);
            }
        }
    }
}
