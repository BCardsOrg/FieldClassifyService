using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Recognition.Classifier.ImageClassifierToolkit.Data
{
    [Serializable]
    public class AcceptanceCriteriaData
    {
        public AcceptanceCriteriaData()
        {
            ModelMinMatchPages = 0.2;
            ModelMaxFpPages = 0.5;
        }

        // Min % of Pages match 
        public double ModelMinMatchPages { get; set; }

        // Max % of Pages FP 
        public double ModelMaxFpPages { get; set; }

    }
}
