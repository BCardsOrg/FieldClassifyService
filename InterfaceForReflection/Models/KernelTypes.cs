using System;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    [Serializable]
    public enum KernelTypes
    {
        Gaussian,
        Polynomial,
        ChiSquare,
        HistogramIntersction
    }
}
