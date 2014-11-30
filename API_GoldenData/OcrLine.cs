using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    [Serializable]
    public class OcrLine
    {
        public OcrLine()
        {
            Words = new List<MappedWord>();
        }
        public Rect Rectangle { get; set; }
        public IList<MappedWord> Words { get; private set; }
        public int ID { get; set; }
      
    }
}
