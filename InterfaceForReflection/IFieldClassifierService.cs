using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using TiS.Recognition.FieldClassifyService.API_GoldenData;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection
{
    public interface IFieldClassifierService
    {
        
        List<int[]> BuildGroupsOfFeatures(string[] chosenFields);
        void InitSelectFeatures(int[] selectFeaturesIn);
         void InitSelectFeatures(string[] selectFeaturesIn);

        void InitFeatureslevel2(List<int[]> groups);
        List<FieldData> GetFieldsOfDoc(string[] feildsin, DocumentData docIn);
        double[] GetFieldFeatures(DocumentData doc, CandidateData fld);
       
        Dictionary<string, int> fieldDic { get; set; }
        int GetDescition(double[] input, out double[] probability);
        void Reset();
        void SaveTrainData(Stream stream, ConfigurationFieldClassifier config, string trainPath, double pageThreshold);
        bool LoadTrainData(Stream stream, out ConfigurationFieldClassifier config, out string trainPath);
        bool IsModelExist { get; }
        double[] NormalizaedConfidence(double[] confIn);
        void CreateDBModelLookUp(string[] Fields, IEnumerable<DocumentData> documents);
        void reduceAmountOfCandidatesForLevel3(int fieldNum, IEnumerable<DocumentData> docs, Action<string> loggerCallBack);

        void PrepareCandidates(IEnumerable<DocumentData> docs, string[] chosenFields, Action<string> loggerCallBack = null);

        void buildModelLevel2(string[] Fields, IEnumerable<DocumentData> docs, List<int[]> groups, InterfaceForReflection.Models.ConfigurationFieldClassifier config, Action<string> loggerCallBack);
        int GetDescitionLevel2(double[] features, out double conf);
        void PrepareCandidateslevel2Test(IEnumerable<DocumentData> docs, int[] optionsperCand, double minConf, Action<string> loggerCallBack);
        void Prepareraindatafalsegrouping(IEnumerable<DocumentData> docs, int[] optionsperCand, double minConf, Action<string> loggerCallBack);
     
        double[] GetsolutionFeatures(solutionData solData, DocumentData doc);
        List<IFieldFeature> LocalFeatures { get; set; }

         double level1RunTestForLevel2(List<DocumentData> TrainData, string[] chosenFields, Action<string> LoggerCallback, List<double> featurescalinglist = null);
         void TuneScaling(double[][] OriginalValues, int[] labels, List<DocumentData> Docs, string[] chosenFields, int classNum, ConfigurationFieldClassifier config, Action<string> LoggerCallback);


          FeatureStatistics CreateModel(string[] Fields,
                                             IEnumerable<DocumentData> documents,
                                             IEnumerable<DocumentData> Testdocuments,
                                             ConfigurationFieldClassifier config,
                                             IEnumerable<string> featursFilter,
                                             Action<string> LoggerCallback,
                                             bool tuneScale,
                                             string pathToScale);

        #region level3
         double[] GetFieldFeatures3(DocumentData doc, CandidateData fld);
     


         void InitFeatureslevel3(string[] fields);
        

         void buildModelLevel3(string[] Fields, IEnumerable<DocumentData> docs, InterfaceForReflection.Models.ConfigurationFieldClassifier config, Action<string> loggerCallBack);
      

         int GetDescition3(double[] input, out double[] probability);

          void insertCompany(string[] companies);
          void getDocFeatures(DocumentData doc, List<double[]> FeatureList, List<int> labels, string[] fields, List<int> featursIndexFilter,
           bool addToStat, FeatureStatistics statOfFeatures, bool usenongoldenForModel, bool getconfidance);
        #endregion
    }
}
