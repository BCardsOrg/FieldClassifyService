using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls.OxyPlot
{
    public class ParemeterAxis : LinearAxis
    {
        static Dictionary<double, string> m_titleMap = new Dictionary<double, string>();

		public const string DayFormat = "Day";
        public static readonly DateTime DateTimeReference = DateTimeAxis.ToDateTime(1) ;

        public ParemeterAxis(AxisPosition pos = AxisPosition.Bottom)
			: base(pos)
		{ }


        public override string FormatValue(double x)
        {
            string title;
            if ( m_titleMap.TryGetValue( x, out title) == true )
            {
                return title;
            }
            else
            {
                return "";
            }
        }

        public static void AddTitle( double index, string title )
        {
            m_titleMap[index] = title;
        }

        public static void Reset()
        {
            m_titleMap.Clear();
        }
    }
}
