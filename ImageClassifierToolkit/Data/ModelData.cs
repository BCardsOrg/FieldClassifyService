using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TiS.Recognition.Classifier.ImageClassifierToolkit.Data
{
    [Serializable]
    public class ModelDataInfo
    {
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public double Match { get; set; }
        public double Reject { get; set; }
        public double FP { get; set; }
    }

    [Serializable]
    public class ModelData : ModelDataInfo
    {
        public Stream ClassifierModel { get; set; }

        public List<PageData> TrainPages { get; set; }

        public AcceptanceCriteriaData AcceptanceCriteria { get; set; }
    }
}
