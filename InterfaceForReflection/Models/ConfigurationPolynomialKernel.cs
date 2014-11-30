using System;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    [Serializable]
    public class ConfigurationPolynomialKernel
    {
        public int Degree { get; set; }

        public double Constant { get; set; }
    }
}
