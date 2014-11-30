using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    [Serializable]
    public class DocumentData
    {
        [NonSerialized]
        DocumentDataNavigation m_wordsNavigator;
        public DocumentDataNavigation WordsNavigator
        {
            get
            {
                if ( m_wordsNavigator == null )
                {
                    m_wordsNavigator = new DocumentDataNavigation(this);
                }
                return m_wordsNavigator;
            }
        }

        [NonSerialized]
        DocumentCandidateNavigation m_candidateNavigator;
        public DocumentCandidateNavigation CandidateNavigator
        {
            get
            {
                if (m_candidateNavigator == null)
                {
                    m_candidateNavigator = new DocumentCandidateNavigation(this);
                }
                return m_candidateNavigator;
            }
        }

        public DocumentData(long idIn)
        {
            DocumentSerialNumber = idIn;
            init();
        }
        public DocumentData()
        {
            init();
        }
        public void init()
        {
            Fields = new List<FieldData>();
            Words = new List<MappedWord>();
            Lines = new List<OcrLine>();
        }

        public long DocumentSerialNumber;
        public string ImageSource { get; set; }
        public Size ImageSize { get; set; }
        public int PageNumber { get; set; }
        public List<FieldData> Fields { get; private set; }

        public List<CandidateData> Candidates { get; set; }
        public List<CandidateData> CandidatesForStage3 { get; set; }

        public List<solutionData> solutionCandidates { get; set; }
        public List<KeyValuePair<double[], int>> solutionSavedFeatures { get; set; }

        public solutionData chosenSolution { get; set; }

        public Dictionary<string,CandidateData> GoldenCandidates { get; set; }

        public Dictionary<string,List<CandidateData>> SpecialCandidates { get; set; }

        public Dictionary<string, CandidateData> lastResultsCandidates { get; set; }
        

        public List<MappedWord> Words { get;  set; }
        public List<OcrLine> Lines { get; private set; }
      public  List<fieldClusterModel> Clusters{get;set;}
        public string DocumentName { get; set; }
        public double RecognitionRate { get; set; }
    }
}
