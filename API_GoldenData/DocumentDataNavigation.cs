using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    [Serializable]
    public class DocumentDataNavigation
    {
        class CompareWords : IEqualityComparer<MappedWord>
        {

            public bool Equals(MappedWord x, MappedWord y)
            {
                return x.Rectangle == y.Rectangle;
            }

            public int GetHashCode(MappedWord obj)
            {
                return obj.Rectangle.GetHashCode();
            }
        }

        const int kGridCellWidth = 100;
        const int kGridCellHeight = 100;

        public DocumentData PageOcr { get; private set; }

        List<List<List<MappedWord>>> m_pageGrid = new List<List<List<MappedWord>>>();

        public DocumentDataNavigation(DocumentData pageOcr)
        {
            PageOcr = pageOcr;

            Init();
        }

        /// <summary>
        /// Get all words that have intersection with the input rect
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public IEnumerable<MappedWord> GetWords(Rect rect)
        {
            int minRowNo = (int)rect.Top / kGridCellHeight;
            int maxRowNo = ((int)rect.Bottom - 1) / kGridCellHeight;
            int minColNo = (int)rect.Left / kGridCellWidth;
            int maxColNo = ((int)rect.Right - 1) / kGridCellWidth;

            var result = m_pageGrid
                    .Where((row, l) => l >= minRowNo && l <= maxRowNo)
                    .SelectMany((row, l) =>row.Where((col, c) => c >= minColNo && c <= maxColNo )
                                              .SelectMany(word => word) )
                    .Where(x => 
                        {
                            return Rect.Intersect(x.Rectangle, rect).IsEmpty == false;
                        })
                    .Distinct(new CompareWords());

            return result;
        }

        private void Init()
        {
            var maxWidth =  Math.Max( PageOcr.ImageSize.Width, PageOcr.Words.Max(x => x.Rectangle.Right) );
            var maxHeight = Math.Max( PageOcr.ImageSize.Height, PageOcr.Words.Max(x => x.Rectangle.Bottom) );

            int numOfRows = ((int)maxHeight - 1) / kGridCellHeight + 1;
            int numOfCols = ((int)maxWidth - 1) / kGridCellWidth + 1;

            for (int i = 0; i < numOfRows; i++)
            {
                m_pageGrid.Add(new List<List<MappedWord>>());
                for (int j = 0; j < numOfCols; j++)
                {
                    m_pageGrid[i].Add(new List<MappedWord>());
                }
            }

            foreach (var word in PageOcr.Words )
            {
                int minRowNo = (int)word.Rectangle.Top / kGridCellHeight;
                int maxRowNo = ((int)word.Rectangle.Bottom - 1) / kGridCellHeight;
                int minColNo = (int)word.Rectangle.Left / kGridCellWidth;
                int maxColNo = ((int)word.Rectangle.Right - 1) / kGridCellWidth;

                for (int iRow = minRowNo; iRow <= maxRowNo; iRow++)
                {
                    for (int iCol = minColNo; iCol <= maxColNo; iCol++)
                    {
                        m_pageGrid[iRow][iCol].Add(word);
                    }
                }
            }
        }
    }
}
