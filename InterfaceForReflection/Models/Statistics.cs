using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
   public class FeatureStatistics
    {
       public List<SingleFieldStatistics> fieldList { get; set; }

      public SingleFieldStatistics notAName { get; set; }

      public List<FieldReportItem> testresults { get; set; }

       public FeatureStatistics()
       {
           fieldList = new List<SingleFieldStatistics>();
           notAName = new SingleFieldStatistics("",-1); 
       }

       public void GetMatchFP(out double Match,out double FP,double ConfidanceIn)
       {
           string[] ChosenFields = fieldList.Select(a => a.name).ToArray();
           double total = 0;
           Match = 0;
           FP = 0;
           foreach (var r in ChosenFields)
           {
               List<FieldReportItem> allResults = testresults.Where(a => (a.ExpectedField.NameFromTypist == r && !a.ExpectedField.IsEmpty)
                   || ((a.ExpectedField.NameFromTypist == r) && (a.ExpectedField.IsEmpty) && (a.MatchQuality > ConfidanceIn))
                  ).ToList();
              total += allResults.Count();
              FP += (double)allResults.Where(a => a.IsFP && a.MatchQuality > ConfidanceIn).Count();
              Match += (double)allResults.Where(a => a.IsMatch && a.MatchQuality > ConfidanceIn ).Count();   
           }

           FP = FP / total * 100;
           Match = Match / total * 100;         
       }

       public void AddFields(string [] Names)
       {
           int index = 0;
           Array.ForEach(Names, a =>
               {
                   fieldList.Add(new SingleFieldStatistics(a, index++));
               });
       }

       public void AddModelFeatures(double[] features,int FieldIndex)
       {
           if ( FieldIndex < 0 )
           {
                notAName.AddFeature(features);
           }
           else
           {
                SingleFieldStatistics field = fieldList.Where(a => a.index == FieldIndex).FirstOrDefault();
                if (field == null) return;
                field.AddFeature(features);
           }
       }

     

       public double GetFeatureFieldGrade(int feature,int fieldindex)
       {
           double finalgrade = 0;
           if (fieldindex < 0) return 0;
           SingleFieldStatistics Field = fieldList.Where(a => a.index == fieldindex).FirstOrDefault();
           double tempgrade = 0;
           double CountResult = fieldList.Where(a=>!a.Equals(Field)).SelectMany(a => a.Features).Count() + notAName.Features.Count;


            
           double FieldmaxSTD = Field.GetAverge(feature) + Field.GetSTD(feature);
           double FieldminSTD = Field.GetAverge(feature) - Field.GetSTD(feature);

           foreach(SingleFieldStatistics singlefield in fieldList)
           {
              
              // tempgrade = Math.Abs(Field.GetAverge(feature) - singlefield.GetAverge(feature)) - (Field.GetSTD(feature) + singlefield.GetSTD(feature)) * singlefield.Features.Count() / CountResult;
               tempgrade += singlefield.Features.Select(a => a[feature]).Select(a => a < FieldminSTD ? 1 : (a > FieldmaxSTD) ? 1 : 0).Sum();

              // finalgrade += tempgrade > 0 ? tempgrade : 0;
           }
           tempgrade += notAName.Features.Select(a => a[feature]).Select(a => a < FieldminSTD ? 1 : (a > FieldmaxSTD) ? 1 : 0).Sum();
          // tempgrade = Math.Abs(Field.GetAverge(feature) - notAName.GetAverge(feature)) - (Field.GetSTD(feature) + notAName.GetSTD(feature)) * notAName.Features.Count() / CountResult; 
          // finalgrade += tempgrade > 0 ? tempgrade : 0;
           finalgrade =Math.Round(tempgrade /  CountResult,2) * 100;
           Field.Grades[feature] = finalgrade;
           return finalgrade;
       }

       public Histograms GetFeatureFieldHistogram(double interval, int feature, SingleFieldStatistics field)
       {
           var maxValue = fieldList.SelectMany(a => a.Features).Select(x => x[feature]).Max();
           var maxnogolden = notAName.Features.Select(x => x[feature]).Max();
           maxValue = Math.Max(maxValue, maxnogolden);

        //   var maxValue = field.Features.Select(x => x[feature]).Max();

           var histogram = new Histograms(10, maxValue);

           foreach (var featureGrade in field.Features.Select(x => x[feature]))
	       {
              histogram.Add(featureGrade);
	       }

         double newMax =   histogram.RemoveEdgeMax();
         var Newhistogram = new Histograms(10, newMax);

           foreach (var featureGrade in field.Features.Select(x => x[feature]))
           {
               Newhistogram.Add(featureGrade);
           }

           return Newhistogram;
       }

       public Histograms GetFeatureFieldHistogram(double interval, int feature,List< SingleFieldStatistics> fields)
       {
           var maxValue = fieldList.SelectMany(a => a.Features).Select(x => x[feature]).Max();
           var maxnogolden = notAName.Features.Select(x => x[feature]).Max();
           maxValue = Math.Max(maxValue, maxnogolden);
               
               //fields.SelectMany(a=>a.Features).Select(x => x[feature]).Max();

           var histogram = new Histograms(10, maxValue);

           foreach (var featureGrade in fields.SelectMany(a => a.Features).Select(x => x[feature]))
           {
               histogram.Add(featureGrade);
           }


           double newMax = histogram.RemoveEdgeMax();
           var Newhistogram = new Histograms(10, newMax);

           foreach (var featureGrade in fields.SelectMany(a => a.Features).Select(x => x[feature]))
           {
               Newhistogram.Add(featureGrade);
           }

           return Newhistogram;
       }


       public double GetFeatureGrade(int feature)
       {
           double grade = fieldList.Select(a => a.Grades[feature]).Average();
           return Math.Round(grade,1);
       }

       public double GetFieldGrade(string field)
       {
           var fieldStat = fieldList.Where(a => a.name == field).FirstOrDefault();

           if (fieldStat != null)
           {
               return Math.Round(fieldStat.Grades.Values.Average(),1);
           }
           else
           {
               return 0;
           }
         
       }
      
       public void NormalizeGrades()
       {
           int NumberOfFeatures = fieldList.ElementAt(0).Features.ElementAt(0).Length;
           for (int i = 0; i < NumberOfFeatures; i++)
           {
               double AverageResult = fieldList.SelectMany(a => a.Features[i]).Average();
               if (AverageResult == 0) AverageResult = 1;
               fieldList.ForEach(a => { Math.Round(a.Grades[i] = a.Grades[i] / AverageResult * 100, 2); });
           }           
       }

       public List<FeatureClassGrade> GetAllGradesSortedFromLow()
       {
           List<FeatureClassGrade> grades = new List<FeatureClassGrade>();
           fieldList.ForEach(a =>
           {
               for (int i = 0; i < a.Grades.Count; i++)
               {
                   grades.Add(new FeatureClassGrade(i, a.name, a.Grades[i]));
               }
           });



           return grades.OrderBy(c=>grades.Where(g=>g.FieldName == c.FieldName).Select(gs=>gs.Grade).Sum()).ThenBy(a => a.FieldName).ThenBy(b=>b.Grade).ToList();
       }

       public double GetFieldSucess(string field)
       {
           if (testresults != null)
           {
               return Math.Round((double)testresults.Where(a => a.Field.NameFromTypist == field && a.IsMatch).Count() / (double)testresults.Where(a => a.ExpectedField.NameFromTypist == field ).Count() * 100,1);
           }
           else
           {
               return 0;
           }
       }
    }

   public class SingleFieldStatistics
   {
       public SingleFieldStatistics(string namein, int indexin)
       {
           name = namein;
           index  = indexin;
           Features = new List<double[]>();
           Grades = new Dictionary<int, double>();
       }

       public double GetAverge(int indexin)
       {
           if (Features.Count <= 0) return 0;
           return Math.Round(Features.Select(a => a[indexin]).Average(),2);
       }
       public double GetSTD(int indexin)
       {
           return Math.Round(Features.Select(a => a[indexin]).STD(), 2);
       }

       public void AddFeature(double[] NewFeature)
       {
           Features.Add(NewFeature);
       }
       public string name { get; set; }
       public int index { get; set; }

       public List<double[]> Features { get; set; }

       public Dictionary<int,double> Grades { get; set; }

   }

   public class FeatureClassGrade
   {
       public FeatureClassGrade(int featidin ,string fieldnamein,double gradin)
       {
           FeatureID = featidin;
           FieldName = fieldnamein;
           Grade = gradin;

       }
       public int FeatureID { get; set; }
       public string FieldName { get; set; }
       public double Grade { get; set; }

   }


}
