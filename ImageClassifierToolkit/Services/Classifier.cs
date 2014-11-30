
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using TiS.Recognition.FieldClassifyService.Service;
using TiS.Recognition.FieldClassifyService.Data;
using TiS.Recognition.FieldClassifyService.Models;
using TiS.Recognition.FieldClassifyService.API_Accord.Models;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services
{
    public static class ClassifierService
    {
        static IFieldClassifierService m_service = new ServiceFieldClassify();

        static public IFieldClassifierService Service
        {
            get
            {
                return m_service;
            }
        }

        static public ConfigurationFieldClassifier GetDefaultConfiguration()
        {
            ConfigurationFieldClassifier config = new ConfigurationFieldClassifier();

            config.FeatureExtraction.NoOfWords = 30;
            config.FeatureExtraction.Extended = false;
            config.FeatureExtraction.UseNonGoldenClass = true;

            config.AccordConfiguration.Kernel = KernelTypes.ChiSquare;

            config.AccordConfiguration.GaussianKernel.Sigma = 6.2;

            config.AccordConfiguration.PolynominalKernel.Constant = 1.0;
            config.AccordConfiguration.PolynominalKernel.Degree = 1;

            config.AccordConfiguration.Complexity = 0.001;
            config.AccordConfiguration.Tolerance = 0.0100;
            config.AccordConfiguration.CacheSize = 500;
            config.AccordConfiguration.SelectionStrategy = SelectionStrategies.WorstPair;

        

            return config;
        }

        public static ConfigurationFieldClassifier GetSetupData(Stream stream, out string trainPath)
        {
            var model = ModelFieldCLassify.Load(stream);
            trainPath = model.TrainPath;

            return model.Configuration;
        }

        public static bool ApplayModel(ModelData modelData)
        {
            if ( modelData == null )
            {
              //  m_service.Reset();
                return true;
            }
            else
            {
                ConfigurationFieldClassifier config;
                string trainPath;

                m_service.LoadTrainData(modelData.ClassifierModel, out config, out trainPath);

               // if ( m_service.LoadTrainData(modelData.ClassifierModel, out config, out trainPath) == true )
                //{
                    // We use zero page threshold becouse here we do not whnat reject pages
                    // We nned it to clculte the final page thresholde
                  //  m_service.SetPageThreshold(0);
                   // return true;
               // }
               // else
               // {
                    return false;
               // }
            }
        }

        public static bool IsModelExist
        {
            get
            {
                return m_service.IsModelExist;
            }
        }

        /// <summary>
        /// Generate config with one diffrent parameter 
        /// </summary>
        public static ConfigurationFieldClassifier GenerateConfiguration(string parameterName,
            double val, ConfigurationFieldClassifier baseConfiguration, out string newParameterValue)
        {
            var configuration = baseConfiguration.Clone() as ConfigurationFieldClassifier;

            newParameterValue = "";

            SetConfiguration(ref newParameterValue, configuration, parameterName, val);

            return configuration;
        }
        public static ConfigurationFieldClassifier GenerateConfiguration(
            Dictionary<string, double> parametersNameVal, ConfigurationFieldClassifier baseConfiguration, out string newParameterValue)
        {
            var configuration = baseConfiguration.Clone() as ConfigurationFieldClassifier;

            newParameterValue = "";

            foreach (var parameterNameVal in parametersNameVal)
            {
                var val = parameterNameVal.Value;

                if (newParameterValue.Length > 0)
                {
                    newParameterValue += ":";
                }

                SetConfiguration(ref newParameterValue, configuration, parameterNameVal.Key, val);
            }

            return configuration;
        }

        private static void SetConfiguration(ref string newParameterValue, ConfigurationFieldClassifier configuration, string parameterName, double val)
        {
          
                    SetConfigurationAccord(ref newParameterValue, configuration, parameterName, val);
         
        }
        

        private static void SetConfigurationAccord(ref string newParameterValue, ConfigurationFieldClassifier configuration, string parameterName, double val)
        {
            switch (parameterName)
            {
                case "NoOfWords":
                    configuration.FeatureExtraction.NoOfWords = Convert.ToInt32(val);
                    newParameterValue += configuration.FeatureExtraction.NoOfWords.ToString();
                    break;

                case "Extended":
                    configuration.FeatureExtraction.Extended = val.ToString() == "1";
                    newParameterValue += configuration.FeatureExtraction.Extended.ToString();
                    break;

                case "Classifier":
                    configuration.AccordConfiguration.Kernel = (KernelTypes)(Convert.ToInt32(val) - 1);
                    newParameterValue += configuration.AccordConfiguration.Kernel.ToString();
                    break;

                case @"Gaussian/Sigma":
                    configuration.AccordConfiguration.GaussianKernel.Sigma = Convert.ToDouble(val);
                    newParameterValue += val.ToString();
                    break;

                case @"Polynomial/Degree":
                    configuration.AccordConfiguration.PolynominalKernel.Degree = Convert.ToInt32(val);
                    newParameterValue += val.ToString();
                    break;

                case @"Polynomial/Constant":
                    configuration.AccordConfiguration.PolynominalKernel.Constant = Convert.ToDouble(val);
                    newParameterValue += val.ToString();
                    break;

                case "Complexity":
                    configuration.AccordConfiguration.Complexity = Convert.ToDouble(val);
                    newParameterValue += val.ToString();
                    break;

                case "Tolerance":
                    configuration.AccordConfiguration.Tolerance = Convert.ToDouble(val);
                    newParameterValue += val.ToString();
                    break;

                case "CacheSize":
                    configuration.AccordConfiguration.CacheSize = Convert.ToInt32(val);
                    newParameterValue += val.ToString();
                    break;

                case "SelectionStrategy":
                    configuration.AccordConfiguration.SelectionStrategy = (SelectionStrategies)(Convert.ToInt32(val) - 1);
                    newParameterValue += configuration.AccordConfiguration.SelectionStrategy.ToString();
                    break;
            }
        }
    }
}
