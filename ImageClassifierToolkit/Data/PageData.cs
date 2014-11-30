using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Recognition.Classifier.ImageClassifierToolkit.Data
{
	[Serializable]
	public class PageData
	{
        public PageData()
        {
            ClassName = string.Empty;
        }
		public string FileName { get; set; }
		
        public int PageNo { get; set; }
		
        public string ClassName { get; set; }
	}
}
