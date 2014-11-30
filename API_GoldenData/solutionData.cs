using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
     [Serializable]
    public class solutionData : IEquatable<solutionData>
    {    
        public solutionData(List<CandidateData> candList)
        {
            offeredSolution = candList;
            Confidance = 0;

        }
        public List<CandidateData> offeredSolution{get;set;}

        public double[] features{get;set;}

        public double Confidance{get;set;}

        public bool Equals(solutionData other)
        {
            if (other.offeredSolution == null || offeredSolution == null) return false;
            if (other.offeredSolution.Count() != offeredSolution.Count()) return false;
            for (int i = 0; i < offeredSolution.Count();i++ )
            {

                if (other.offeredSolution.ElementAt(i) != offeredSolution.ElementAt(i)) return false;
               

            }

                return true;
        }
    }
}
