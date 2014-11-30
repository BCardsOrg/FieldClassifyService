using System;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    [Serializable]
    public class ConfigurationFeatureExtraction
    {
        public int NoOfWords { get; set; }

        public bool Extended { get; set; }

        public bool UseNonGoldenClass { get; set; }
    }
}
