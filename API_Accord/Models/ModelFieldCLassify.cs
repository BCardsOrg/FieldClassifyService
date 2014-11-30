using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

namespace TiS.Recognition.FieldClassifyService.API_Accord.Models
{

    [Serializable]
    public class ModelFieldCLassify : PersistentEntity<ModelFieldCLassify>
    {
        #region Ctor
        public ModelFieldCLassify()
        {
            Bow = new MemoryStream();
            Ksvm = new MemoryStream();
            TrainImageFeatureVectors = new MemoryStream();
            ClassIdClassNameMap = new MemoryStream();
        }
        #endregion

        #region Public properties

        // <Feature Name, Feature scale (-1 = use default scale)
        public Dictionary<string, double> FeaturesScale { get; set; }

        public Stream Bow { get; set; }

        public Stream Ksvm { get; set; }

        public Stream TrainImageFeatureVectors { get; set; }

        public Stream ClassIdClassNameMap { get; set; }

        public ConfigurationFieldClassifier Configuration { get; set; }

        public string TrainPath { get; set; }

        public double PageThreshold { get; set; }

        #endregion
    }
}
