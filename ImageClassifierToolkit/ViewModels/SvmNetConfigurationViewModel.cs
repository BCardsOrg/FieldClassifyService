
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
    public class SvmNetConfigurationViewModel : BaseConfigurationViewModel
    {
        public SvmNetConfigurationViewModel()
        {
            RegisterProperty("C", NotifyGroup.Configuration);
            RegisterProperty("CacheSize", NotifyGroup.Configuration);
            RegisterProperty("Coefficient0", NotifyGroup.Configuration);
            RegisterProperty("Degree", NotifyGroup.Configuration);
            RegisterProperty("EPS", NotifyGroup.Configuration);
            RegisterProperty("EpsilonP", NotifyGroup.Configuration);
            RegisterProperty("Gamma", NotifyGroup.Configuration);
            RegisterProperty("Nu", NotifyGroup.Configuration);
        }

        public double C
        {
            get
            {

                return 0; // AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.C;
            }
            set
            {
               // OnChange(C, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.C = x);
            }
        }

        public double CacheSize
        {
            get
            {
                return 0; // AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.CacheSize;
            }
            set
            {
               // OnChange(CacheSize, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.CacheSize = x);
            }
        }
        public double Coefficient0
        {
            get
            {
                return 0;// AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.Coefficient0;
            }
            set
            {
               // OnChange(Coefficient0, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.Coefficient0 = x);
            }
        }
        public int Degree
        {
            get
            {
                return 0;// AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.Degree;
            }
            set
            {
               // OnChange(Degree, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.Degree = x);
            }
        }
        public double EPS
        {
            get
            {
                return 0;// AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.EPS;
            }
            set
            {
               // OnChange(EPS, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.EPS = x);
            }
        }
        public double EpsilonP
        {
            get
            {
                return 0;// AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.EpsilonP;
            }
            set
            {
               // OnChange(EpsilonP, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.EpsilonP = x);
            }
        }
        public double Gamma
        {
            get
            {
                return 0;// AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.Gamma;
            }
            set
            {
              //  OnChange(Gamma, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.Gamma = x);
            }
        }

        public double Nu
        {
            get
            {
                return 0; //AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.Nu;
            }
            set
            {
                // OnChange(Nu, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.SvmNetConfiguration.Nu = x);
            }
        }

    }
}

