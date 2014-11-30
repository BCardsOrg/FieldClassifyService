using System.Collections.Generic;
//using TiS.Recognition.ImageClassifier.Image.Imaging;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    public class Page
    {
        public Page()
        {
            MatchData = new PageMatchData();
        }
       // public UnmanagedImage Image { get; set; }

        public IEnumerable<double> Features { get; set; }

        public PageMatchData MatchData { get; private set; }
    }
}
