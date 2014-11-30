using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiS.Recognition.FieldClassifyService.API_GoldenData;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection
{
    public interface ISolutionFeature
    {
        double[] GetFeature(solutionData solutionOption, DocumentData doc);

        bool IsSelected { get; set; }
      
        string Name { get;  }
        
    }
   
}
