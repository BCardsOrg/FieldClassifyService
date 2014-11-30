using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    [System.Diagnostics.DebuggerDisplay("{Name} - {Contents} - {Rectangle} - {Confidence}")]
    [Serializable]
    public class FieldData : OcrWord
    {
        public FieldData()
        {
            GotMatched = false;
        }
        public string Name { get; set; }
        public bool GotMatched { get; set; }

        public double[] AccordConfidance { get; set; }

    }
}
