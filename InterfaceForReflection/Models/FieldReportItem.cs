using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiS.Recognition.FieldClassifyService.API_GoldenData;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    public class FieldReportItem
    {

        public FieldReportItem(string name, int fieldIndex, CandidateData candidate, DocumentData docData)
        {
            MatchedName = name;
            IsMatch = false;
            IsFP = false;
            // MatchQuality = 0;
            m_fieldIndex = fieldIndex;
            Field = candidate;
            Doc = docData;

            Update();
        }
        private int m_fieldIndex;

        //public string Name{get;private set;}
        public string MatchedName { get; private set; }
        public bool IsMatch { get; private set; }

        void Update()
        {
            ExpectedField = Doc.Candidates.Where(b => b.NameFromTypist == MatchedName).FirstOrDefault();

            if (Field.NameFromTypist != MatchedName)
            {
                // need to change when we decide to find only some of the fields
                if (ExpectedField == null)
                {
                    IsMatch = false;
                    IsFP = true;
                }
                else if (Field.Content != ExpectedField.Content)
                {
                    IsMatch = false;
                    IsFP = true;
                }
            }
            else
            {
                IsMatch = true;
                IsFP = false;
            }

            if (ExpectedField == null)
            {
                ExpectedField = new CandidateData();
                ExpectedField.NameFromTypist = MatchedName;
                //expectedField.FinalConfidance = 0;
            }

        }
        public bool IsFP { get; private set; }
        public bool IsRejected
        {
            get
            {
                return IsMatch == false && IsFP == false;
            }
        }

        public double MatchQuality
        {
            get
            {
                if (Field != null && Field.AccordConfidance != null)
                {
                    return Math.Round(Field.AccordConfidance[m_fieldIndex] * 100, 1);
                }
                else
                {
                    return 0;
                }
            }
        }

        public double ExpectedMatchQuality
        {
            get
            {
                if (ExpectedField != null && ExpectedField.AccordConfidance != null)
                {
                    return Math.Round(ExpectedField.AccordConfidance[m_fieldIndex] * 100,1);
                }
                else
                {
                    return 0;
                }
            }
        }


        public  DocumentData Doc { get; private set; }

        // The best mach candidate
        public CandidateData Field { get; private set; }

        // Other candidates that match this field  (order by confidence)
        public IEnumerable<CandidateData> OtherFields { get; set; }

        // The gold candidate
        public CandidateData ExpectedField { get; private set; }
    }
}
