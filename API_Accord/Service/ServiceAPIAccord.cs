using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Controls;
using Accord.Math;
using Accord.Statistics.Kernels;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TiS.Recognition.FieldClassifyService.API_Accord.Models;
using TiS.Recognition.FieldClassifyService.API_GoldenData;
using System.Threading;
using System.Diagnostics;


using ZedGraph;

namespace TiS.Recognition.FieldClassifyService.API_Accord 
{
    public class ServiceAPIAccord 
    {

      PerformanceCounter  cpuCounter = new PerformanceCounter();



    
        
            public string getCurrentCpuUsage(){
                return cpuCounter.NextValue()+"%";
            }



        #region tests
        //MultilabelSupportVectorMachine machine;
        ISupportVectorMachine m_machine;
        ISupportVectorMachineLearning m_teacher;

        bool _usenongoldenclass = false;
       
       // IKernel kernel;


   

        public double BuildTheModel(double[][] inputs, int[] outputs, int ClassNum, ConfigurationFieldClassifier config)
        {
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";


            Reset();
            _usenongoldenclass = config.FeatureExtraction.UseNonGoldenClass;
          //  scalers = scalresin;

            IKernel kernal = null;

            switch (config.AccordConfiguration.Kernel)
            {
                case KernelTypes.Gaussian:
                    kernal = new Gaussian(config.AccordConfiguration.GaussianKernel.Sigma);
                    break;

                case KernelTypes.Polynomial:
                    kernal = new Polynomial(config.AccordConfiguration.PolynominalKernel.Degree, config.AccordConfiguration.PolynominalKernel.Constant);
                    break;

                case KernelTypes.ChiSquare:
                    kernal = new ChiSquare();
                    break;

                case KernelTypes.HistogramIntersction:
                    kernal = new HistogramIntersection();
                    break;

                default:
                    break;
            }
            
      
          
            if (ClassNum > 2)
            {
                m_machine = new MulticlassSupportVectorMachine(inputs[0].Length, kernal, ClassNum);
       
                m_teacher = (new MulticlassSupportVectorLearning((MulticlassSupportVectorMachine)m_machine, inputs, outputs));
       
                (m_teacher as MulticlassSupportVectorLearning).Algorithm = (svm, classInputs, classOutputs, i, j) =>
            {
                var smo = new SequentialMinimalOptimization(svm, classInputs, classOutputs);
                smo.Complexity = config.AccordConfiguration.Complexity;
                smo.Tolerance = config.AccordConfiguration.Tolerance;
                smo.CacheSize = config.AccordConfiguration.CacheSize;
                smo.Strategy = (Accord.MachineLearning.VectorMachines.Learning.SelectionStrategy)((int)(config.AccordConfiguration.SelectionStrategy));
                    //  smo.UseComplexityHeuristic = true;
                    //  smo.PositiveWeight = 1;

                int k = 0;
                while (cpuCounter.NextValue() > 50)
                {
                    Thread.Sleep(50);
                    k++;
                    if(k > 30000)
                    {
                        break;
                    }
                }
                // smo.NegativeWeight = 1;
              

                smo.Run();
                var probabilisticOutputLearning = new ProbabilisticOutputLearning(svm, classInputs, classOutputs);
                return probabilisticOutputLearning; 

               // return smo;
            };

            }
            else
            {
                // FIX TO BASE TYPES THAN RUN THAN MAKE OTHER 2 CHANGES from LATEST  - line ... and CLUSTER AND RUNTEST.. and check again... 

                m_machine = new SupportVectorMachine(inputs[0].Length);
                m_teacher = new SequentialMinimalOptimization((SupportVectorMachine)m_machine, inputs, outputs);
                (m_teacher as SequentialMinimalOptimization).Complexity = config.AccordConfiguration.Complexity; ;
                (m_teacher as SequentialMinimalOptimization).Tolerance = config.AccordConfiguration.Tolerance;
                (m_teacher as SequentialMinimalOptimization).CacheSize = config.AccordConfiguration.CacheSize;
                (m_teacher as SequentialMinimalOptimization).Strategy = (Accord.MachineLearning.VectorMachines.Learning.SelectionStrategy)((int)(config.AccordConfiguration.SelectionStrategy));
                (m_teacher as SequentialMinimalOptimization).Complexity = config.AccordConfiguration.Complexity; ;
            }
       


            // Configure the learning algorithm
           

            // Train the machines. It should take a while.
         //   Thread.Sleep(10000);
//#if temp
            double error = m_teacher.Run();

//#endif
         //   return 0;

         
           

            return error;   

        }




        public int Getdecision(double[] input, out double[] probability)
        {
           /* if (m_scalers != null)
            { 
                foreach (var it in m_scalers.Select((x, i) => new { Value = x, Index = i }))
            {
               input[it.Index] = it.Value.getScaled(input[it.Index]);
            }             
            }*/
           
                
                int Error = ((m_machine as MulticlassSupportVectorMachine).Compute(input, out probability));
                if (_usenongoldenclass)
                {
                    probability = probability.ToList().Take(probability.Count() - 1).ToArray();
                }

                return Error;
        }

        public int GetDecisionLevel2(double[] input, out double probability)
        {
            return ((m_machine as SupportVectorMachine).Compute(input, out probability));
        }



        #endregion

        #region Private fields
       

        //BinarySplit m_bs = new BinarySplit(numberOfWords);
        private double[][] m_trainImageFeatureVectors = null;

        private IDictionary<int, string> m_classIdClassNameMap = new Dictionary<int, string>();

        private double m_pageThreshold = 0;
        // Create bag-of-words (BoW) with the given number of words
     //   private BagOfVisualWords<CornerFeaturePointEx> m_bow = null;
        #endregion

        #region Public methods
        public void SaveTrainData(Stream stream, ConfigurationFieldClassifier config, string trainPath, double pageThreshold, IEnumerable<IFieldFeature> features)
        {
            var model = new ModelFieldCLassify();

            model.PageThreshold = pageThreshold;
            
         //   m_bow.Save(model.Bow);
            (m_machine as MulticlassSupportVectorMachine).Save(model.Ksvm);

            //BinaryFormatter f = new BinaryFormatter();
            //f.Serialize(model.TrainImageFeatureVectors, m_trainImageFeatureVectors);
            //f.Serialize(model.ClassIdClassNameMap, m_classIdClassNameMap);

            model.Configuration = config;
            model.TrainPath = trainPath;
            model.FeaturesScale = features.ToDictionary(x => x.Name, y => y.FieldScale);

            ModelFieldCLassify.Save(model, stream);
        }

        public bool IsModelExist
        {
            get
            {
              //  return m_bow != null && m_ksvm != null;
                return m_machine != null;
            }
       }

        public bool LoadTrainData(Stream stream, out ConfigurationFieldClassifier config, out string trainPath, IEnumerable<IFieldFeature> features)
        {
            try
            {
                var model = ModelFieldCLassify.Load(stream);
                model.Ksvm.Position = 0;

                m_machine = MulticlassSupportVectorMachine.Load(model.Ksvm);
              //  model.Bow.Position = 0;
              //  m_bow = BagOfVisualWords.Load<CornerFeaturePointEx>(model.Bow);
                //BinaryFormatter f = new BinaryFormatter();
                //model.TrainImageFeatureVectors.Position = 0;
                //m_trainImageFeatureVectors = f.Deserialize(model.TrainImageFeatureVectors) as double[][];
                //model.ClassIdClassNameMap.Position = 0;
                //m_classIdClassNameMap = f.Deserialize(model.ClassIdClassNameMap) as Dictionary<int, string>;
                config = model.Configuration;
                trainPath = model.TrainPath;
                m_pageThreshold = model.PageThreshold;
                if ( model.FeaturesScale != null && model.FeaturesScale.Count > 0 )
                {
                    foreach (var featue in features)
                    {
                        double scale;
                        if ( model.FeaturesScale.TryGetValue(featue.Name, out scale) == true )
                        {
                            featue.FieldScale = scale;
                        }
                    }
                }
            }
            catch
            {
                Reset();
                config = null;
                trainPath = string.Empty;
                return false;
            }
            return true;
        }

        public void Reset()
        {
          //  m_bow = null;
            m_machine = null;
            m_teacher = null;
            m_trainImageFeatureVectors = null;
            m_classIdClassNameMap = new Dictionary<int, string>();
        }

        public void Train(IEnumerable<Page> Docs,
            ConfigurationFieldClassifier config)
        {
            Reset();

           // SetPagesFeatures(config, pages);

            // Create the learning algorithm using the machine and the training data

           // m_classIdClassNameMap = GetClassIdClassNameMap(pages);

           // var numberOfClasses = m_classIdClassNameMap.Count;
           // var outputs = pages.Select(x => x.MatchData.ClassId).ToArray();
           // var fv = pages.Select(x => x.Features.ToArray()).ToArray();

            //BuildModel(config, numberOfClasses, outputs, fv);
        }

        public void TrainUnknown(IEnumerable<Page> pages, ConfigurationFieldClassifier config, Func<bool> isAbort)
        {
            Reset();

            // First set each page as different class
            for (int i = pages.Count() - 1; i >= 0; i--)
            {
                pages.ElementAt(i).MatchData.ClassId = i;
            }

            SetPagesFeatures(config, pages);

            // Create the learning algorithm using the machine and the training data
            foreach (var page in pages)
            {
                if (isAbort() == true)
                {
                    return;
                }

                // Skip page that found the class
                if (pages.Where(x => x != page)
                          .Any(x => x.MatchData.ClassId == page.MatchData.ClassId) == true)
                {
                    continue;
                }

                var trainPages = pages.Where(x => x != page).ToList();


                var numberOfClasses = NormalizeClassNamesUnknown(trainPages);
                var outputs = trainPages.Select(x => x.MatchData.ClassId).ToArray();
                var fv = trainPages.Select(x => x.Features.ToArray()).ToArray();

            //    BuildModel(config, numberOfClasses, outputs, fv);

                double output1;
                var paths1 = new Tuple<int, int>[3];
                page.MatchData.ClassId = (m_machine as MulticlassSupportVectorMachine).Compute(page.Features.ToArray(), out output1, out paths1);

                page.MatchData.Confidence = output1;
                page.MatchData.DecisionPath = paths1;
            }
        }

        public void Run(Page page, ConfigurationFieldClassifier config, Action<IDictionary<string, double>> pageMatch)
        {
          //  page.Features = m_bow.GetFeatureVector(page.Image);

            double output1;
            var paths1 = new Tuple<int, int>[3];
            var recClassId = (m_machine as MulticlassSupportVectorMachine).Compute(page.Features.ToArray(), out output1, out paths1);

            output1 = System.Math.Abs(output1);

            string recClassName;
            if (m_classIdClassNameMap.TryGetValue(recClassId, out recClassName) == false)
            {
                recClassName = string.Format("__Class{0}__", recClassId);
            }

            if (output1 >= m_pageThreshold) //Current implementation only fills highest graded class as output (output1). Dictionary supports more than 1 class match, assuming sort by match score desc.
            {
                pageMatch(new Dictionary<string, double>() { { recClassName, output1 } });
            }
            else
            {
                pageMatch(new Dictionary<string, double>());
            }
        }

        public void SetPageThreshold(double pageThreshold)
        {
            m_pageThreshold = pageThreshold;
        }
        #endregion


      
        public void ReplaceImageWithSvmData(ConfigurationFieldClassifier config, Page page)
        {
           /* if (m_bow == null)
            {
                // Calculate bow...
                BinarySplit bs = new BinarySplit(config.FeatureExtraction.NoOfWords);

                // Create bag-of-words (BoW) with the given number of words
                m_bow = new BagOfVisualWords<CornerFeaturePointEx>(
                    new CornerDetectorWithDiagonalLines(new CornerFeaturesDetector(new HarrisCornersDetector())), bs);
            }

            page.Image.SvmData = m_bow.GetImageSvmData(page.Image);
            page.Image.DisposeImageData();*/
        }

        private void SetPagesFeatures(ConfigurationFieldClassifier config, IEnumerable<Page> pages)
        {
            // Set Page.Features...
          /* if (m_bow == null)
            {
                // Calculate bow...
                BinarySplit bs = new BinarySplit(config.FeatureExtraction.NoOfWords);

                // Create bag-of-words (BoW) with the given number of words
                m_bow = new BagOfVisualWords<CornerFeaturePointEx>(
                    new CornerDetectorWithDiagonalLines(new CornerFeaturesDetector(new HarrisCornersDetector())), bs);

                // Compute the BoW codebook using training images only
                var points = m_bow.Compute(pages.Select(x => x.Image).ToArray());

                // Update page with 

                Parallel.ForEach(pages, p =>
                {
                    p.Features = m_bow.GetFeatureVector(p.Image);
                });

                m_trainImageFeatureVectors = pages.Select(x => x.Features.ToArray()).ToArray();
            }
            else
            {
                for (int i = 0; i < m_trainImageFeatureVectors.Length; i++)
                {
                    pages.ElementAt(i).Features = m_trainImageFeatureVectors[i];
                }
            }*/
        }


        // Set all ClassId to be from 0 to #Classes - 1
        private int NormalizeClassNamesUnknown(IEnumerable<Page> pages)
        {
            int newClassName = -1;
            int oldClassName = -1;
            int noOfClasses = 0;
            foreach (var page in pages.OrderBy(x => x.MatchData.ClassId).ToList())
            {
                if (page.MatchData.ClassId != oldClassName)
                {
                    oldClassName = page.MatchData.ClassId;
                    newClassName++;
                    noOfClasses++;
                }
                page.MatchData.ClassId = newClassName;
            }

            return noOfClasses;
        }
    
    
    }
}
