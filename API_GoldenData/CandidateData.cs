using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    [System.Diagnostics.DebuggerDisplay("{Content} - {Rectangle}")]
    [Serializable]
    public class CandidateData
    {
        public static char[] Seperators = new char[] { ' ' };
        public string NameFromTypist { get; set; }

        public double[] AccordConfidance { get; set; }
        public double[] AccordConfidance3 { get; set; }

        public double[] Features { get; set; }
        public double[] Features3 { get; set; }

         List<MappedWord> _Words = null;



         public IEnumerable<MappedWord> Words
        {
            get
            {
                return _Words;

            }
        }

        string m_content = "";
        public string Content
        {
            get
            {
                return m_content;
            }
        }

        string m_contentNoSpace = "";
        public string ContentNoSpace
        {
            get
            {
                return m_contentNoSpace;
            }
        }

        public void EnterContentWithFix(string contentIn)
        {
            string[] contentWords = contentIn.Split(Seperators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < contentWords.Length;i++ )
            {
                if (_Words.Count() >= i) return;
                _Words.ElementAt(i).Contents = contentWords[i];
            }

        }

        public int NumOfLines { get; set; }
       

        public Rect Rectangle
        {
            get
            {
                if (IsEmpty == true) return Rect.Empty;
                return  Words.Select(a=>a.Rectangle).Aggregate((i, j) => Rect.Union( i,j));

            }

        }

        public Rect OriginalRectangle
        {
            get
            {
                if (IsEmpty == true) return Rect.Empty;
                return Words.Select(a => a.OriginRectangle).Aggregate((i, j) => Rect.Union(i, j));

            }

        }

        public bool IsEmpty
        {
            get
            {
                return Words == null;
            }
        }

        public void AddWords(IEnumerable<MappedWord> list)
        {
            if ( list == null || list.Count() == 0 )
            {
                _Words = null;
            }
            else
            {
                if (_Words == null)
                {
                    _Words = new List<MappedWord>();
                }
                _Words.AddRange(list);
            }
            CalcContent();
        }

        private void CalcContent()
        {
            if ( IsEmpty == true )
            {
                m_content = "";
                m_contentNoSpace = "";
            }
            else
            {
                m_content = _Words.Select(a => a.Contents).Aggregate((i, j) => (i + " " + j).TrimEnd()).TrimEnd(new char[] { ',' });
                m_contentNoSpace = _Words.Select(a => a.Contents).Aggregate((i, j) => (i + j).TrimEnd()).TrimEnd(new char[] { ',' });
            }
        }
    }
}
