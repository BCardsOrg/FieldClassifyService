using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    [Serializable]
    public class OcrWord
    {
        public int ID { get; set; }
        public string Contents { get; set; }
        public Rect Rectangle { get; set; }
        public Rect OriginRectangle { get; set; }
        public int Confidence { get; set; }
        public OcrLine Line { get; set; }


        public override string ToString()
        {
            return "'" + Contents + "'";
        }

       
    }
}
