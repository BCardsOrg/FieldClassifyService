
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class ParameterRange
    {
        public bool IsSelected { get; set; }
        public string ParameterName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Interval { get; set; }
        public bool IsEnum { get; set; }
    }

    public class MultiRunConfigurationViewModel
    {
        public IEnumerable<ParameterRange> Parameters { get; private set; }

        public MultiRunConfigurationViewModel()
        {
           // switch (FactoryServices.SvmEngineType)
           // {
              //  case SvmEngine.Svm:
                    Parameters = new List<ParameterRange>() 
                    {
                        new ParameterRange() { ParameterName="NoOfWords", From="10", To="40", Interval="2", IsEnum=false, IsSelected=true },
                        new ParameterRange() { ParameterName="Extended", From="0", To="1", Interval="1", IsEnum=false},
                        new ParameterRange() { ParameterName="Classifier", From="1", To="4", Interval="1", IsEnum=true},
                        new ParameterRange() { ParameterName=@"Gaussian/Sigma", From="1", To="20", Interval="1", IsEnum=false},
                        new ParameterRange() { ParameterName=@"Polynomial/Degree", From="0", To="5", Interval="1", IsEnum=false},
                        new ParameterRange() { ParameterName=@"Polynomial/Constant", From="0.2", To="2", Interval="0.2", IsEnum=false},
                        new ParameterRange() { ParameterName="Complexity", From="0.0005", To="0.010", Interval="0.0005", IsEnum=false},
                        new ParameterRange() { ParameterName="Tolerance", From="0.005", To="0.1", Interval="0.005", IsEnum=false},
                        new ParameterRange() { ParameterName="CacheSize", From="300", To="700", Interval="50", IsEnum=false},
                        new ParameterRange() { ParameterName="SelectionStrategy", From="1", To="2", Interval="1", IsEnum=true},
                    };
               //     break;
              //  case SvmEngine.SvmNet:
               //     Parameters = new List<ParameterRange>() 
               //     {
               //         new ParameterRange() { ParameterName="NoOfWords", From="10", To="40", Interval="2", IsEnum=false, IsSelected=true },
               //         new ParameterRange() { ParameterName="Extended", From="0", To="1", Interval="1", IsEnum=false},
               //         new ParameterRange() { ParameterName="Coefficient0", From="1", To="8", Interval="2", IsEnum=false},
               //         new ParameterRange() { ParameterName="C", From="1", To="4", Interval="1", IsEnum=false},
               //         new ParameterRange() { ParameterName="Degree", From="0", To="5", Interval="1", IsEnum=false},
                //        new ParameterRange() { ParameterName="Nu", From="1", To="8", Interval="2", IsEnum=false},
               //         new ParameterRange() { ParameterName="Gamma", From="0", To="4", Interval="1", IsEnum=false},
               //         new ParameterRange() { ParameterName="CacheSize", From="10", To="100", Interval="20", IsEnum=false},
               //         new ParameterRange() { ParameterName="EPS", From="0", To="0.1", Interval="0.02", IsEnum=false},
               //         new ParameterRange() { ParameterName="EpsilonP", From="0", To="1", Interval="0.2", IsEnum=false},
               //     };
               //     break;
           // }
            
        }
    }
}
