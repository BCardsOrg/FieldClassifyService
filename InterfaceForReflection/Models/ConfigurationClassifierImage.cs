using System;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    [Serializable]
    public class ConfigurationFieldClassifier : ICloneable
    {
        #region Ctor
        public ConfigurationFieldClassifier()
        {
            AccordConfiguration = new AccordConfigurationClassifier();

            FeatureExtraction = new ConfigurationFeatureExtraction();
        }
        #endregion

        public AccordConfigurationClassifier AccordConfiguration { get; set; }

        public ConfigurationFeatureExtraction FeatureExtraction { get; set; }

        public object Clone()
        {
            return PersistentEntity<ConfigurationFieldClassifier>.Clone(this);
        }
    }
}
