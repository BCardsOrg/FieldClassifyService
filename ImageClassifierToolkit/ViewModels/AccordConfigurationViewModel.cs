using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class AccordConfigurationViewModel : BaseConfigurationViewModel
	{
        public AccordConfigurationViewModel()
        {
            RegisterProperty("Kernel", NotifyGroup.Configuration);
            RegisterProperty("Sigma",  NotifyGroup.Configuration);
            RegisterProperty("Constant",  NotifyGroup.Configuration);
            RegisterProperty("Degree",  NotifyGroup.Configuration);
            RegisterProperty("Complexity",  NotifyGroup.Configuration);
            RegisterProperty("Tolerance",  NotifyGroup.Configuration);
            RegisterProperty("CacheSize",  NotifyGroup.Configuration);
            RegisterProperty("SelectionStrategy",  NotifyGroup.Configuration);
        }

        public KernelTypes Kernel
        {
            get
            {

                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.Kernel;
            }
            set
            {
                OnChange(Kernel, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.Kernel = x);
            }
        }

        public double Sigma
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.GaussianKernel.Sigma;
            }
            set
            {
                OnChange(Sigma, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.GaussianKernel.Sigma = x);
            }
        }
        public double Constant
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.PolynominalKernel.Constant;
            }
            set
            {
                OnChange(Constant, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.PolynominalKernel.Constant = x);
            }
        }
        public int Degree
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.PolynominalKernel.Degree;
            }
            set
            {
                OnChange(Degree, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.PolynominalKernel.Degree = x);
            }
        }
        public double Complexity
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.Complexity;
            }
            set
            {
                OnChange(Complexity, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.Complexity = x);
            }
        }
        public double Tolerance
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.Tolerance;
            }
            set
            {
                OnChange(Tolerance, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.Tolerance = x);
            }
        }
        public int CacheSize
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.CacheSize;
            }
            set
            {
                OnChange(CacheSize, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.CacheSize = x);
            }
        }
        public SelectionStrategies SelectionStrategy
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.SelectionStrategy;
            }
            set
            {
                OnChange(SelectionStrategy, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.AccordConfiguration.SelectionStrategy = x);
            }
        }

    }
}

