using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiS.Recognition.Common;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection;
using TiS.Recognition.FieldClassifyService.API_GoldenData;
using TiS.Recognition.FieldClassifyService;
using TiS.Recognition.FieldClassifyService.Service;
using TiS.Recognition.FieldClassifyService.Models;
using System.IO;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using TiS.Recognition.FieldClassifyService.Data;
using System.Runtime.Serialization.Formatters.Binary;
using modulesForComm;

namespace FieldClassifyLibraryForService
{
    public class MainModule
    {

       static  ServiceFieldClassify ClassifyService = new ServiceFieldClassify();

       static ModelData modelData;
   

        public static void PerformClassification(string tifFileName,double minConfOut, out ClassifyResult resultClassify,out string resultfilename)
        {
            List<DocumentData> docs = new List<DocumentData>();
        
            DocumentData doc;
            DocumentsLoader.GetAllDocuemntPRDTIFOnly(tifFileName, out doc);
                docs.Add(doc);
            ClassifyService.PrepareCandidates(docs,modelData.FieldsSelectedNames.ToArray());

            ClassifyService.getDocFeatures(doc, null, null, null, new List<int>(), false, null, true, false);

            ClassifyService.level1RunTestForLevel2(docs, modelData.FieldsSelectedNames.ToArray(), AddConsoleMessage);

            resultClassify = new ClassifyResult();

            resultClassify.Items = new ClassifyResultField[modelData.FieldsSelectedNames.ToArray().Length];

            foreach (var r in modelData.FieldsSelectedNames.ToArray().Select((x, i) => new { Value = x, Index = i }))
            {
                List<CandidateData> candidatesforField = docs[0].Candidates.OrderByDescending(a => a.AccordConfidance[r.Index]).Take(4).Where(b => b.AccordConfidance[r.Index] > minConfOut).ToList();
                resultClassify.Items[r.Index] = new ClassifyResultField();
                resultClassify.Items[r.Index].id = r.Value;
                if (candidatesforField.Any())
                {
                    resultClassify.Items[r.Index].candidates = new ClassifyResultFieldCandidatesCandidate[candidatesforField.Count];
                    for (int i = 0; i < candidatesforField.Count; i++)
                    {
                        resultClassify.Items[r.Index].candidates[i] = new ClassifyResultFieldCandidatesCandidate();
                        resultClassify.Items[r.Index].candidates[i].value = candidatesforField[i].Content;
                        resultClassify.Items[r.Index].candidates[i].confidance = candidatesforField[i].AccordConfidance[r.Index].ToString();
                    }
                }
            }


          


            var fileName = Path.GetFileNameWithoutExtension(tifFileName); 
                    var dirName = Path.GetDirectoryName(tifFileName);
                    resultfilename = Path.Combine(dirName, fileName + ".xml");

                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ClassifyResult));
                    
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(resultfilename))
                        {
                            writer.Serialize(file, resultClassify);
                           
                        }
                    
           

        }


        public static void AddConsoleMessage(string message)
        {
           
        }

        //temporary 
        public static void initFieldClassifier()
        {
                               
          
            ConfigurationFieldClassifier config;
            string trainPath;


             BinaryFormatter f = new BinaryFormatter();

             using (var stream = File.OpenRead(Repository.GetModelFileName()))
            {
                modelData =  f.Deserialize(stream) as ModelData;
            }

             ClassifyService.InitSelectFeatures(modelData.FeaturesSelectedNames.ToArray());

             ClassifyService.LoadTrainData(modelData.ClassifierModel, out config, out trainPath);

           
        }
    }
}
