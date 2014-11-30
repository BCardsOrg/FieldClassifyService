using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiS.Recognition.FieldClassifyService.API_GoldenData;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection
{
    public enum FeatureImportant
    {
        Low = 1,
        Normal = 2,
        High = 3
    }

    public interface IFieldFeature
    {
        double GetFeature(CandidateData field, DocumentData doc);
        double GetFeatureNoScale(CandidateData field, DocumentData doc);
        bool IsSelected { get; set; }
        string Name { get; }
        double FieldScale { get; set; }
        FeatureImportant Important { get; }
         double? minFeatureValueOnTraining { get; set; }
    }

}
