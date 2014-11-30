using System;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    [Serializable]
    public class AccordConfigurationClassifier
    {
        #region Ctor

        public AccordConfigurationClassifier()
        {
            GaussianKernel = new ConfigurationGaussianKernel();
            PolynominalKernel = new ConfigurationPolynomialKernel();
        }
        #endregion

        #region Public Properties

        public KernelTypes Kernel { get; set; }

        public ConfigurationGaussianKernel GaussianKernel { get; set; }

        public ConfigurationPolynomialKernel PolynominalKernel { get; set; }

        public double Complexity { get; set; }

        public double Tolerance { get; set; }

        public int CacheSize { get; set; }

        public SelectionStrategies SelectionStrategy { get; set; }
        #endregion
    }
}

