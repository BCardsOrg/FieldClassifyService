using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
   public class ReportResultItem
    {
       public ReportResultItem(string namein)
       {
           _name = namein;
           _ResultHist = new Dictionary<string, int>();
           _ResultHist[_name] = 0;
       }
       string _name;
         Dictionary<string, int> _ResultHist;
       public double TotalPrecentage
       {

           get
           {
               if (!_ResultHist.ContainsKey(_name)) return 0;
               double total = (double)_ResultHist.Select(a => a.Value).Aggregate((a, i) => a + i);
               if (total == 0) return 0;
               return Math.Round((double)_ResultHist[_name] / total * 100, 2);
           }
           set { }
       }
       public string Name {
           get { return _name; }
           set { _name = value; }
       }

       public Dictionary<string, int> ResultHist { get { return _ResultHist; } set { _ResultHist = value; } }

       public void AddResult(string namein)
       {
           if (!_ResultHist.ContainsKey(namein)) _ResultHist[namein] = 1;
           else _ResultHist[namein]++;
       }
       
    }
}
