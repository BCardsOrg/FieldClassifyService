using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.ObjectModel;

using System.Threading.Tasks;
using System.Threading;
using TiS.Recognition.FieldClassifyService;
using TiS.Recognition.FieldClassifyService.Service;
using System.Diagnostics;
using System.Collections.Generic;
using FieldClassifyLibraryForService;

namespace Test_Classify
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestReadCollections()
        {

            string tifFileName = @"C:\ELI\OCR\test\candidates\1\deskew.tif";
            FieldClassifyLibraryForService.Models.ClassifyResult result = new FieldClassifyLibraryForService.Models.ClassifyResult();

            FieldClassifyLibraryForService.MainModule.initFieldClassifier();
            string pathresult = "";
            FieldClassifyLibraryForService.MainModule.PerformClassification(tifFileName, 0, out result, out pathresult);

          /*  var startTime = DateTime.Now;

            List<DocumentData> docCollections = AppDataCenter.Singleton.TrainPages.Select(a => a.docData).ToList();


            var selectedFeatures = AppDataCenter.Singleton.FeaturesSelected.Select((a, i) => new { item = a, index = i }).Where(a => a.item.IsSelected).Select(a => a.index).ToArray();
            //  selectedFeatures = new int[] {  37 };
            string[] FieldsToTest = AppDataCenter.Singleton.FieldsSelected.Where(a => a.IsChecked == true).Select(a => a.Name).ToArray();
            ClassifierService.Service.InitSelectFeatures(selectedFeatures);
            // AppDataCenter.Singleton.AddConsoleMessage("Building Model");

            ClassifierService.Service.CreateModel(FieldsToTest
                , docCollections, FieldsToTest.Length, Configuration, AppDataCenter.Singleton.AddConsoleMessage);
            //   AppDataCenter.Singleton.AddConsoleMessage("Buidling Model Done");

            double confidenceOut;




            List<ReportResultItem> ReoprtResults = new List<ReportResultItem>();
            Array.ForEach(FieldsToTest,
                new Action<string>(a =>
                {
                    ReoprtResults.Add(new ReportResultItem(a));
                }));



            AppDataCenter.Singleton.RejectedFields.Clear();
            ConcurrentBag<FieldReportItem> fieldBag = new ConcurrentBag<FieldReportItem>();

            foreach (DocumentData doc1 in AppDataCenter.Singleton.SetupData.Pages.Where(a => AppDataCenter.Singleton.TrainPages.Contains(a) == false).Select(a => a.docData))
            {
                //     AppDataCenter.Singleton.AddConsoleMessage("Testing Page : " + docnum++);

                List<FieldData> fields = ClassifierService.Service.GetFieldsOfDoc(FieldsToTest, doc1);

                foreach (FieldData field in fields)
                {
                    FieldReportItem fieldReportItem = new FieldReportItem();
                    fieldReportItem.Name = field.Name;
                    fieldReportItem.field = field;
                    fieldReportItem.doc = doc1;


                    Tuple<int, int>[] DecitionTree;
                    int result = 0;

                    result = ClassifierService.Service.GetDescition(ClassifierService.Service.GetFieldFeatures(doc1, field), out confidenceOut, out DecitionTree);
                    fieldReportItem.MatchedName = FieldsToTest[result];





                    if ((confidenceOut * 100) > AppDataCenter.Singleton.AcceptanceCriteria.ProbablityCreteria)
                    {
                        fieldReportItem.isMatch = true;
                        fieldReportItem.MatchQuality = confidenceOut * 100;

                        //   ReportResultItem reportItem = ReoprtResults.FirstOrDefault(a => a.Name == field.Name);
                        // if (reportItem != null)
                        //  {

                        //     reportItem.AddResult(FieldsToTest[result]);
                        //  }
                        // else
                        //  {
                        //      throw new NotImplementedException();
                        //   }
                        //  numberOfTests++;

                        // CorrectResultsTotal += field.Name == FieldsToTest[result] ? 1 : 0;
                        if (field.Name != fieldReportItem.MatchedName)
                        {
                            fieldReportItem.isMatch = false;
                            fieldReportItem.isFP = true;

                        }
                    }
                    else
                    {
                        fieldReportItem.isRejected = true;
                        //  Models.CustomFieldData rejectedField =   new Models.CustomFieldData();
                        //  rejectedField.doc = doc1;
                        //   rejectedField.fieldData = field;


                        //   AppDataCenter.Singleton.RejectedFields.Add(rejectedField);   
                    }
                    //   Trace.WriteLine(field.Name == FieldsToTest[result] ? "" : " ERROR");
                    fieldBag.Add(fieldReportItem);


                }
            }

            ReportData.noOfPages = fieldBag.Count();
            ReportData.fpCount = fieldBag.Where(a => a.isFP).Count();
            ReportData.matchCount = fieldBag.Where(a => a.isMatch).Count();
            ReportData.rejectCount = fieldBag.Where(a => a.isRejected).Count();




            foreach (FieldReportItem fieldresult in fieldBag.Where(a => a.isMatch))
            {
                ReportData.QualityMatch.Add(fieldresult.MatchQuality);
                ReportResultItem reportItem = ReoprtResults.FirstOrDefault(a => a.Name == fieldresult.Name);
                reportItem.AddResult(fieldresult.MatchedName);
            }

            foreach (FieldReportItem fieldresult in fieldBag.Where(a => a.isFP))
            {
                ReportResultItem reportItem = ReoprtResults.FirstOrDefault(a => a.Name == fieldresult.Name);
                reportItem.AddResult(fieldresult.MatchedName);
            }


            foreach (FieldReportItem fieldresult in fieldBag.Where(a => a.isRejected))
            {
                Models.CustomFieldData rejectedField = new Models.CustomFieldData();
                rejectedField.doc = fieldresult.doc;
                rejectedField.fieldData = fieldresult.field;
                AppDataCenter.Singleton.RejectedFields.Add(rejectedField);
            }

            ReportData.Duration = DateTime.Now - startTime;
            AppDataCenter.Singleton.AddReportData(ReportData);*/
                
        }


        [TestMethod]
        public void TestBuildModel()
        {

          /*  Machine_SequentialMinimalOptimization trainer1 = new Machine_SequentialMinimalOptimization();

            double[][] inputs =
            {
                new double[] { 0 , 1 },
                new double[] { 0 , 15 },
                new double[] { 0  ,30}
               
            };

            // Outputs for each of the inputs 
            int[] outputs =
            {
                0 ,
              1 ,
               2
              
            };

            trainer1.BuildModelLinear(inputs, outputs, 3, 1);
            double[] confidence;
            int result;
            result = trainer1.Getdecision(inputs[2], out confidence);*/

        }

    }
}
