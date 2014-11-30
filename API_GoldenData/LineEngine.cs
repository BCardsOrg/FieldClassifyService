using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiS.Recognition.FieldClassifyService.API_GoldenData;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
   public class LineEngine
    {
       public static readonly double  PrecentAsLine = 0.5;
       public static List<FieldClusterLine> CreateLines(DocumentData doc, fieldClusterModel cluster)
       {
           DocumentDataNavigation docNavigate = doc.WordsNavigator;

           var wordList = cluster.Fields;


           var wordHeight = CalcWordHeight(wordList);










           List<FieldClusterLine> result = new List<FieldClusterLine>();
           wordList = wordList.OrderBy(a => a.Rectangle.Top).ToList();
           List<MappedWord> newWordList = new List<MappedWord>(wordList);

           Dictionary<MappedWord, List<MappedWord>> wordLines = new Dictionary<MappedWord, List<MappedWord>>();

           foreach (var word in newWordList)
           {
               var inTheSameLine = docNavigate.GetWords(new System.Windows.Rect(cluster.Area.Left, word.Rectangle.Top, cluster.Area.Width, word.Rectangle.Height))
                   .Where(x => x.Cluster.ID == word.Cluster.ID && x.Clusterline == null)
                   .Where( x => x == word || x.Rectangle.Top + (x.Rectangle.Height/2)*1.2  < word.Rectangle.Bottom )
                   .ToList();

               if ( inTheSameLine.Count == 0)
               {
                   throw new Exception("A) No words in word line");
               }

               wordLines.Add(word, inTheSameLine);
           }


           int index = 0;
           while (newWordList.Count > 0)
           {
               MappedWord WordTop = newWordList.FirstOrDefault(x => x.Rectangle.Height < wordHeight * 2);

               if ( WordTop == null )
               {
                   WordTop = newWordList.First();
               }

               var baseWords = wordLines[WordTop].Where( x => x.Clusterline == null ).ToList();

               if ( baseWords.Count == 0)
               {
                   // Bad word
                   throw new Exception("B) No words in word line");
               }

               FieldClusterLine FieldLine = new FieldClusterLine();
               FieldLine.ID = index++;
               FieldLine.Fields = wordLines.Where(x => baseWords.Contains(x.Key))
                   .SelectMany( x => x.Value )
                   .Where( x => x.Clusterline == null )
                   .Distinct()
                   .OrderBy(x => x.Rectangle.Left)
                   .ToList();
                   
               newWordList.RemoveAll(a => FieldLine.Fields.Contains(a));



               //FieldLine.Fields = newWordList.Where(a => WordTop.Line.Words.Contains(a)).ToList().OrderBy(b => b.Rectangle.X).ToList();

             //  FieldLine.Fields = newWordList.Where(a => (a.Rectangle.Top <= (WordTop.Rectangle.Top + (WordTop.Rectangle.Height * PrecentAsLine)))).ToList().OrderBy(b=>b.Rectangle.X).ToList();

               FieldLine.Fields.ForEach(a => a.Clusterline = FieldLine);
               result.Add(FieldLine);
           }


           return result;
       }

       // Calc the common workd height form list of words
       private static double CalcWordHeight(IEnumerable<MappedWord> wordList)
       {
           var validWords = wordList.Where(x => x.Rectangle.Width > x.Rectangle.Height).ToList();
           if ( validWords.Count > 0 )
           {
               return validWords.Average(x => x.Rectangle.Height);
           }
           else
           {
               return wordList.Average(x => x.Rectangle.Height);
           }
       }
    }
}
