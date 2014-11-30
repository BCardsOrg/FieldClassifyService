using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using System.Threading.Tasks;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Dialogs;
using System.Collections.Concurrent;
using System.Windows.Media.Imaging;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

using System.Collections.ObjectModel;
using TiS.Recognition.FieldClassifyService.API_GoldenData;
using System.Diagnostics;

using System.IO;
using TiS.Recognition.FieldClassifyService.Service;
using TiS.Recognition.FieldClassifyService.Common;
using TiS.Recognition.FieldClassifyService.Models;
//using TiS.Core.TisCommon;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    public class TrainRunCommand : CommandBaseAsyncUI
    {
        bool m_cancelRun;

        List<string> m_featuresCalculate;
        bool m_useLastRuntimeData;

        public TrainRunCommand() :
            base(Shell.ShellDispatcher)
        {
            Configuration = AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration;
            ReportData = new ReportData();
            TuneScale = false;
            RunOnTrainData = false;
        }

        public bool TuneScale { get; set; }
        public bool RunOnTrainData { get; set; }

        public ConfigurationFieldClassifier Configuration { get; set; }

        public ReportData ReportData { get; set; }
        //  string[] FieldsToTest { get;set;}
        protected override void LoadData(object state)
        {
            // true if we only calculate the filter feature and all other calculate data we take from previous run
            if (AppDataCenter.Singleton.IsAllFeaturesCalculate == false)
            {
                m_featuresCalculate = AppDataCenter.Singleton.FeaturesSelected
                                                .Where(x => AppDataCenter.Singleton.IsFeatureCalculate(x.Name) == true)
                                                .Select(x => x.Name)
                                                .ToList();
                m_useLastRuntimeData = true;
            }
            else
            {
                m_featuresCalculate = new List<string>();
                m_useLastRuntimeData = false;
            }

            bool subRunCommand = AppDataCenter.Singleton.IsRunning;

            if (subRunCommand == false)
            {
                AppDataCenter.Singleton.StartAnalyzer();
            }

            try
            {
                var startTime = DateTime.Now;
               

               
                List<DocumentData> AllData = AppDataCenter.Singleton.SetupData.Pages.Select(a => a.DocData).Where(b => b.Words.Count >= 0).ToList();

                AppDataCenter.Singleton.ChosenFields = AppDataCenter.Singleton.SetupData.FilteredFields.Where(a => a.IsSelected == true)
                                                                                       .OrderByDescending(a => a.NumApear)
                                                                                       .Select(a => a.Name).ToArray();


                if (m_useLastRuntimeData == false)
                {
                    ClassifierService.Service.PrepareCandidates(AllData, AppDataCenter.Singleton.ChosenFields, AppDataCenter.Singleton.AddConsoleMessage);
                }

                ClassifierService.Service.insertCompany(AllData.SelectMany(a => a.GoldenCandidates.Where(b => b.Value.NameFromTypist == "fCompanyName")).Select(b => b.Value.Content).ToArray());

                //    AllData = AllData.Where(a => FieldsToTest.All(b => a.Candidates.Select(c => c.NameFromTypist).Contains(b))).ToList();
                //  AllData = AllData.Where(a => FieldsToTest.All(b => a.Fields.Where(f=>f.Name!=null && f.Rectangle.IsEmpty == false).Select(c => c.Name).Contains(b))).ToList();

                List<DocumentData> TrainData = AppDataCenter.Singleton.TrainPages.Select(a => a.DocData).Where(b => b.Words.Count >= 0).ToList();
                List<DocumentData> TestData;
                //  TrainData = TrainData.Where(a => FieldsToTest.All(b => a.Candidates.Select(c => c.NameFromTypist).Contains(b))).ToList();
                if ( RunOnTrainData == true )
                {
                    TestData = TrainData;
                }
                else
                {
                    TestData = AllData.Where(a => TrainData.Contains(a) == false).ToList();
                    //TestData = TestData.Where(x => x.ImageSource == @"E:\Files\Projects\FieldClassifier\Boaz\UK\30_invoices_Vendors\7C1D341B-80E3-4331-9116-A9DFA1F91A18.TIF").ToList();
                }

                Level1Classification(TrainData, TestData);
                //UpdateFeatureGrades();
                ReportResults();


                AppDataCenter.Singleton.AddReportData(ReportData);
                ReportData.Duration = DateTime.Now - startTime;

            //    ClassifierService.Service.reduceAmountOfCandidatesForLevel3(AppDataCenter.Singleton.ChosenFields.Length, AllData, AppDataCenter.Singleton.AddConsoleMessage);
           //    Level3Classification(TrainData, TestData);


             //   Level2Classification(TrainData, TestData);






            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (subRunCommand == false)
                {
                    AppDataCenter.Singleton.FinishAnalyzer();
                }
            }
        }

        public void Level2Classification(List<DocumentData> TrainData, List<DocumentData> TestData)
        {
            List<int[]> groups = ClassifierService.Service.BuildGroupsOfFeatures(AppDataCenter.Singleton.ChosenFields);


            int[] optionsPerSol = new int[AppDataCenter.Singleton.ChosenFields.Length];

            for (int i = 0; i < optionsPerSol.Length; i++) { if (groups.SelectMany(a => a).Contains(i)) optionsPerSol[i] = 2; else  optionsPerSol[i] = 1; }

            ClassifierService.Service.InitFeatureslevel2(groups);

            ClassifierService.Service.Prepareraindatafalsegrouping(TrainData, optionsPerSol, 0, AppDataCenter.Singleton.AddConsoleMessage);

            // prepare candidates for not group with test Data and move them into the build 

            ClassifierService.Service.buildModelLevel2(AppDataCenter.Singleton.ChosenFields, TrainData, groups, Configuration, AppDataCenter.Singleton.AddConsoleMessage);


            ClassifierService.Service.PrepareCandidateslevel2Test(TestData, optionsPerSol, 0, AppDataCenter.Singleton.AddConsoleMessage);

            TestData.ForEach(doc =>
            {
                solutionData chosenSolution = null;
                double lastProb = -100;
                doc.solutionCandidates.ToList().ForEach(
                    sol =>
                    {
                        double conf;
                        ClassifierService.Service.GetDescitionLevel2(sol.features, out conf);
                        sol.Confidance = conf;
                        if (conf > lastProb)
                        {
                            chosenSolution = sol;
                            lastProb = conf;
                        }
                    }
                    );
                doc.chosenSolution = chosenSolution;
            });


            ConcurrentBag<FieldReportItem> fieldBag = new ConcurrentBag<FieldReportItem>();
            int index = 0;

            double TotalGolden = 0;
            double TotalMatched = 0;

            foreach (DocumentData doc1 in TestData)
            {
                var startTimeFeatures = DateTime.Now;

                TimeSpan FeatureCalcSpan = new TimeSpan(0);
                double NumGolden = 0;
                double NumMatched = 0;


                foreach (var r in AppDataCenter.Singleton.ChosenFields.Select((x, i) => new { Value = x, Index = i }))
                {
                 //   if (doc1.Candidates.Where(a => a.NameFromTypist == r.Value).ToList().Count() <= 0) continue;
                    NumGolden++;


                    FieldReportItem fieldReportItem = new FieldReportItem(r.Value, r.Index, doc1.chosenSolution.offeredSolution[r.Index], doc1);

                    if (fieldReportItem.IsMatch) NumMatched++;
                    /*  }
                      else
                      {
                          fieldReportItem.isRejected = true;

                      }*/

                    Trace.WriteLine("Doc : " + doc1.DocumentName + ",Field : " + r.Value + " ,Recognized : " + fieldReportItem.MatchedName + " content : " + doc1.chosenSolution.offeredSolution[r.Index].Content + "Match : " + (fieldReportItem.IsMatch ? " True" : " False"));



                    fieldBag.Add(fieldReportItem);

                }
                TotalGolden += NumGolden;
                TotalMatched += NumMatched;
                AppDataCenter.Singleton.AddConsoleMessage(index++ + " Page : " + doc1.DocumentName + " Getting Features (sec) : " + FeatureCalcSpan.TotalSeconds + " success : " + Math.Round(NumMatched / NumGolden * 100) + "%");
            }
            AppDataCenter.Singleton.AddConsoleMessage(" Total Matched :  " + Math.Round(TotalMatched / TotalGolden * 100,1) + "%");



            // calculate success

            //     print result
        }



        private void Level1Classification(List<DocumentData> TrainData, List<DocumentData> TestData)
        {
            var selectedFeatures = AppDataCenter.Singleton.FeaturesSelected.Select((a, i) => new { item = a, index = i })
                                                                           .Where(a => a.item.IsSelected).Select(a => a.index).ToArray();
            //  selectedFeatures = new int[] {  37 };
          
           

            ClassifierService.Service.InitSelectFeatures(selectedFeatures);



            ScalingFactors scales = null;

          //  if(AppDataCenter.Singleton.PathToScale != null)
           // {
           //      scales = FeatureListHolder.loadScalesFromFile(ClassifierService.Service.LocalFeatures, AppDataCenter.Singleton.PathToScale);

              
           // }
            // AppDataCenter.Singleton.AddConsoleMessage("Building Model");
            AppDataCenter.Singleton.ChosenFeatures = ClassifierService.Service.LocalFeatures;
            AppDataCenter.Singleton.UpdateFeatureVertHorzSelection();
            AppDataCenter.Singleton.EntireModelStats = ClassifierService.Service.CreateModel(AppDataCenter.Singleton.ChosenFields
            , TrainData, TestData, Configuration, m_featuresCalculate, AppDataCenter.Singleton.AddConsoleMessage, TuneScale, AppDataCenter.Singleton.PathToScale);


          //  if (AppDataCenter.Singleton.PathToScale != null)
           // {
             

              //  FeatureListHolder.SaveScalesToFile(ClassifierService.Service.LocalFeatures, AppDataCenter.Singleton.PathToScale,scales);
           // }
           

            double[] confidenceOut;

            List<ReportResultItem> ReoprtResults = new List<ReportResultItem>();
            Array.ForEach(AppDataCenter.Singleton.ChosenFields,
                new Action<string>(a =>
                {
                    ReoprtResults.Add(new ReportResultItem(a));
                }));
            ReportResultItem NullResult = new ReportResultItem("");
            ReoprtResults.Add(NullResult);

            AppDataCenter.Singleton.RejectedFields.Clear();

            ClassifierService.Service.level1RunTestForLevel2(TrainData,AppDataCenter.Singleton.ChosenFields,AppDataCenter.Singleton.AddConsoleMessage);


            // add printing of train data sucss

            ConcurrentBag<FieldReportItem> fieldBag = new ConcurrentBag<FieldReportItem>();


            AppDataCenter.Singleton.AddConsoleMessage("Running Test on  : " + TestData.Count + " Pages");
            int index = 0;
           // int result;
            foreach (DocumentData doc1 in TestData)
            {
                var startTimeFeatures = DateTime.Now;

                //     AppDataCenter.Singleton.AddConsoleMessage("Testing Page : " + docnum++);

                //  List<FieldData> fields = ClassifierService.Service.GetFieldsOfDoc(FieldsToTest, doc1);
                TimeSpan FeatureCalcSpan = new TimeSpan(0);

               /* foreach (CandidateData field in doc1.Candidates)
                {
                    result = 0;
                    startTimeFeatures = DateTime.Now;
                    if (m_useLastRuntimeData == true)
                    {
                        foreach (var featureIndex in ClassifierService.Service.LocalFeatures
                                                        .Select((x, i) => AppDataCenter.Singleton.IsFeatureCalculate(x.Name) == true ? i : -1)
                                                        .Where(x => x >= 0))
                        {
                            field.Features[featureIndex] = ClassifierService.Service.GetFieldFeature(doc1, field, featureIndex);
                        }
                    }
                    else
                    {
                        field.Features = ClassifierService.Service.GetFieldFeatures(doc1, field);
                    }
                    FeatureCalcSpan += (DateTime.Now - startTimeFeatures);
                    result = ClassifierService.Service.GetDescition(field.Features, out confidenceOut);


                  //  field.AccordConfidance = (ClassifierService.Service.NormalizaedConfidence(confidenceOut));
                    field.AccordConfidance = (confidenceOut);
                }*/
                 List<int> featursIndexFilter = new List<int>();
            // Get the index of the feature we need to calculate - all other features will be use the value already exist in the field

                 ClassifierService.Service.getDocFeatures(doc1, null, null, AppDataCenter.Singleton.ChosenFields, featursIndexFilter, false, null, true, true);

                double NumGolden = 0;
                double NumMatched = 0;

                foreach (var r in AppDataCenter.Singleton.ChosenFields.Select((x, i) => new { Value = x, Index = i }))
                {

                  //  if (doc1.Candidates.Where(a => a.NameFromTypist == r.Value).ToList().Count() <= 0) continue;
                    NumGolden++;

                    CandidateData fieldCandidate = doc1.Candidates.OrderByDescending(a => a.AccordConfidance[r.Index]).FirstOrDefault();

                    FieldReportItem fieldReportItem = new FieldReportItem(r.Value, r.Index, fieldCandidate, doc1);
                  

                    if (fieldReportItem.IsMatch) NumMatched++;
                    /*  }
                      else
                      {
                          fieldReportItem.isRejected = true;

                      }*/
                    if (!fieldReportItem.IsMatch)
                    Trace.WriteLine("Doc : " + doc1.DocumentName + " ,Confidance : " + fieldCandidate.AccordConfidance[r.Index] + ",Field : " + r.Value + " ,Recognized : " + fieldCandidate.NameFromTypist + " content : " + fieldCandidate.Content + (fieldReportItem.IsMatch ? " True" : " False"));



                    fieldBag.Add(fieldReportItem);

                }

                AppDataCenter.Singleton.AddConsoleMessage(index++ + " Page : " + doc1.DocumentName + " Getting Features (sec) : " + FeatureCalcSpan.TotalSeconds + " success : " + Math.Round(NumMatched / NumGolden * 100) + "%");
            }



            AppDataCenter.Singleton.AddConsoleMessage("Update results");


            AppDataCenter.Singleton.EntireModelStats.testresults = fieldBag.ToList();


            ReportData.noOfFields = fieldBag.Count();
            ReportData.NoOfPages = TestData.Count;
            ReportData.fpCount = fieldBag.Where(a => a.IsFP).Count();
            ReportData.matchCount = fieldBag.Where(a => a.IsMatch).Count();
            ReportData.rejectCount = fieldBag.Where(a => a.IsRejected).Count();

            foreach (FieldReportItem fieldresult in fieldBag.Where(a => a.IsMatch))
            {
                ReportData.QualityMatch.Add(fieldresult.MatchQuality);

            }

            List<FieldReportItem> FPList = fieldBag.Where(a => a.IsFP).ToList().OrderBy(b => b.Field.NameFromTypist ?? "").ThenByDescending(a => a.Field.AccordConfidance.Max()).ToList();


            foreach (var r in AppDataCenter.Singleton.ChosenFields)
            {
                List<FieldReportItem> FPfromGolden = FPList.Where(a => a.ExpectedField.NameFromTypist == r).ToList();
                List<FieldReportItem> FPnotFromGolden = FPList.Where(a => a.ExpectedField.NameFromTypist != r && a.MatchedName == r).ToList();
                double success = (double)fieldBag.Where(a => a.Field.NameFromTypist == r && a.IsMatch).Count() / (double)fieldBag.Where(a => a.Field.NameFromTypist == r ).Count() * 100;
                int countNum = fieldBag.Where(a => a.Field.NameFromTypist == r ).Count();
                Trace.WriteLine("*********** The field : " + r + " success : " + Math.Round(success,1) + "%   Count : " + countNum + "         *************");
            }
            AppDataCenter.Singleton.NotifyChange(NotifyGroup.ScatterData);
        }



        private void Level3Classification(List<DocumentData> TrainData, List<DocumentData> TestData)
        {

            AppDataCenter.Singleton.AddConsoleMessage("\nStarting Level3 ***************\n");


            ClassifierService.Service.InitFeatureslevel3(AppDataCenter.Singleton.ChosenFields);


            ClassifierService.Service.buildModelLevel3(AppDataCenter.Singleton.ChosenFields
            , TrainData, Configuration, AppDataCenter.Singleton.AddConsoleMessage);


            ConcurrentBag<FieldReportItem> fieldBag = new ConcurrentBag<FieldReportItem>();

            AppDataCenter.Singleton.AddConsoleMessage("Getting results from level 1 to be used in level 3");

            TestData.AsParallel().ForAll(doc =>
            {
                doc.lastResultsCandidates = new Dictionary<string, CandidateData>();
                for (int i = 0; i < AppDataCenter.Singleton.ChosenFields.Length; i++)
                {
                    CandidateData releventCandidate = doc.Candidates.Where(a => a.AccordConfidance[i] == doc.Candidates.Select(b => b.AccordConfidance[i]).Max()).FirstOrDefault();
                    doc.lastResultsCandidates.Add(AppDataCenter.Singleton.ChosenFields[i], releventCandidate);
                }

            });

            AppDataCenter.Singleton.AddConsoleMessage("Running Test on  : " + TestData.Count + " Pages");
            int index = 0;
            int result;
            double[] confidenceOut;

        


            foreach (DocumentData doc1 in TestData)
            {
                var startTimeFeatures = DateTime.Now;

            
                TimeSpan FeatureCalcSpan = new TimeSpan(0);

                foreach (CandidateData field in doc1.CandidatesForStage3)
                {
                  
                    {
                        field.Features3 = ClassifierService.Service.GetFieldFeatures3(doc1, field);
                    }
                    FeatureCalcSpan += (DateTime.Now - startTimeFeatures);
                    result = ClassifierService.Service.GetDescition3(field.Features3, out confidenceOut);


                    //  field.AccordConfidance = (ClassifierService.Service.NormalizaedConfidence(confidenceOut));
                    field.AccordConfidance3 = (confidenceOut);
                }

                double NumGolden = 0;
                double NumMatched = 0;

                foreach (var r in AppDataCenter.Singleton.ChosenFields.Select((x, i) => new { Value = x, Index = i }))
                {

                    //  if (doc1.Candidates.Where(a => a.NameFromTypist == r.Value).ToList().Count() <= 0) continue;
                    NumGolden++;

                    CandidateData fieldCandidate = doc1.CandidatesForStage3.OrderByDescending(a => a.AccordConfidance[r.Index]).Take(7).OrderByDescending(a => a.AccordConfidance3[r.Index]).FirstOrDefault();

                    FieldReportItem fieldReportItem = new FieldReportItem(r.Value, r.Index, fieldCandidate, doc1);


                    if (fieldReportItem.IsMatch) NumMatched++;
                    /*  }
                      else
                      {
                          fieldReportItem.isRejected = true;

                      }*/
                    if (!fieldReportItem.IsMatch)
                        Trace.WriteLine("Doc : " + doc1.DocumentName + " ,Confidance : " + fieldCandidate.AccordConfidance3[r.Index] + ",Field : " + r.Value + " ,Recognized : " + fieldCandidate.NameFromTypist + " content : " + fieldCandidate.Content + (fieldReportItem.IsMatch ? " True" : " False"));



                    fieldBag.Add(fieldReportItem);

                }

                AppDataCenter.Singleton.AddConsoleMessage(index++ + " Page : " + doc1.DocumentName + " Getting Features (sec) : " + FeatureCalcSpan.TotalSeconds + " success : " + Math.Round(NumMatched / NumGolden * 100,1) + "%");
            }



            AppDataCenter.Singleton.AddConsoleMessage("Update results");


           // AppDataCenter.Singleton.EntireModelStats.testresults = fieldBag.ToList();


            double noOfFields = fieldBag.Count();
            
            double fpCount = fieldBag.Where(a => a.IsFP).Count();
            double matchCount = fieldBag.Where(a => a.IsMatch).Count();
            double rejectCount = fieldBag.Where(a => a.IsRejected).Count();

           

            List<FieldReportItem> FPList = fieldBag.Where(a => a.IsFP).ToList().OrderBy(b => b.Field.NameFromTypist ?? "").ThenByDescending(a => a.Field.AccordConfidance.Max()).ToList();


            foreach (var r in AppDataCenter.Singleton.ChosenFields)
            {
                List<FieldReportItem> FPfromGolden = FPList.Where(a => a.ExpectedField.NameFromTypist == r).ToList();
                List<FieldReportItem> FPnotFromGolden = FPList.Where(a => a.ExpectedField.NameFromTypist != r && a.MatchedName == r).ToList();
                double success = (double)fieldBag.Where(a => a.Field.NameFromTypist == r && a.IsMatch).Count() / (double)fieldBag.Where(a => a.Field.NameFromTypist == r).Count() * 100;
                int countNum = fieldBag.Where(a => a.Field.NameFromTypist == r).Count();
                Trace.WriteLine("*********** The field : " + r + " success : " + Math.Round(success,1) + "%   Count : " + countNum + "         *************");
            }


            AppDataCenter.Singleton.AddConsoleMessage("Total result level 3 , sucsss : " + Math.Round(matchCount / noOfFields * 100, 2) + "%");
        }


        //private void LookUpClassification(List<DocumentData> TrainData, List<DocumentData> TestData)
        //{


        //    ClassifierService.Service.CreateDBModelLookUp(AppDataCenter.Singleton.ChosenFields, TrainData);

        //    ConcurrentBag<FieldReportItem> fieldBag2 = new ConcurrentBag<FieldReportItem>();

        //    foreach (DocumentData doc1 in TestData)
        //    {
        //        //     AppDataCenter.Singleton.AddConsoleMessage("Testing Page : " + docnum++);



        //        foreach (CandidateData field in doc1.Candidates)
        //        {

        //            if (!field.GotMatched)
        //            {
        //                FieldReportItem fieldReportItem = new FieldReportItem(field.NameFromTypist, AppDataCenter.Singleton.ChosenFields.IndexOf( x=> x == field.NameFromTypist ));
        //                //fieldReportItem.Name = field.NameFromTypist;
        //                fieldReportItem.field = field;
        //                fieldReportItem.doc = doc1;



        //                string resultName = null;
        //                if ((resultName = ClassifierService.Service.GetFieldResultFromLookUp(field)) != null)
        //                {
        //                    fieldReportItem.MatchedName = resultName;
        //                    fieldReportItem.isMatch = true;
        //                    //fieldReportItem.MatchQuality = field.AccordConfidance.Max() * 100;
        //                    field.GotMatched = true;

        //                    if (field.NameFromTypist != fieldReportItem.MatchedName)
        //                    {

        //                        fieldReportItem.isMatch = false;
        //                        fieldReportItem.isFP = true;

        //                    }
        //                }
        //                else
        //                {
        //                    //  fieldReportItem.isRejected = true;
        //                }

        //                if ((field.NameFromTypist != null) || (field.GotMatched))
        //                {
        //                    fieldBag2.Add(fieldReportItem);
        //                }


        //            }


        //        }




        //    }
        //    // ReportData.noOfPages = fieldBag.Count();
        //    ReportData.fpCount += fieldBag2.Where(a => a.isFP).Count();
        //    ReportData.matchCount += fieldBag2.Where(a => a.isMatch).Count();
        //    ReportData.rejectCount -= fieldBag2.Where(a => (!a.isRejected)).Count();
        //}

        private void ReportResults()
        {


            AppDataCenter.Singleton.AddConsoleMessage(string.Format("***  Match  :  {0} %", Math.Round((double)ReportData.matchCount / (double)ReportData.noOfFields * 100, 2)));
            AppDataCenter.Singleton.AddConsoleMessage(string.Format("***  Reject  :  {0} %", Math.Round((double)ReportData.rejectCount / (double)ReportData.noOfFields * 100, 2)));
            AppDataCenter.Singleton.AddConsoleMessage(string.Format("***  FP  :  {0} %", Math.Round((double)ReportData.fpCount / (double)ReportData.noOfFields * 100, 2)));
            AppDataCenter.Singleton.AddConsoleMessage(string.Format("***  Sucess Of Passed creteria  :  {0} %", Math.Round((double)ReportData.matchCount / ((double)ReportData.matchCount + (double)ReportData.FP) * 100, 2)));



        }

        protected override void UpdateView(object state)
        {
        }



        protected override void UpdateData(object state)
        {
        }
    }
}
