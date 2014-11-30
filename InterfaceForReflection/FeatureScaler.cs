using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection
{
  public  class FeatureScaler
    {
        public double minValue { get; set; }
        public double maxValue { get; set; }
        public double AVG { get; set; }
        public double STD { get; set; }
        public double scale = 1;

        public double getScaled (double value)
        {
         // return value;
           if (STD <= 0) return value;
          // return (value ) / STD;
            if (minValue >= maxValue) return value;

          //  return (value - minValue) / STD;
            return ((value - minValue) * 100 / (maxValue - minValue)) * scale;
        }
    }
}
