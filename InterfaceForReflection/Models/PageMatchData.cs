using System;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    public class PageMatchData
    {
        public int ClassId { get; set; }

        public double Confidence { get; set; }

        public Tuple<int, int>[] DecisionPath { get; set; }
    }
}
