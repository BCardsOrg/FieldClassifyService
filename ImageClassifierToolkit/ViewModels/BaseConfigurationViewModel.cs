
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
using TiS.Recognition.FieldClassifyService.InterfaceForReflection;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;


namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class FeatureViewModel : NotifyPropertyChanged
    {
        public FeatureViewModel(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }

        bool m_isSelected;
        public bool IsSelected
        {
            get
            {
                return m_isSelected;
            }
            set
            {
                if (OnChange(ref m_isSelected, value, "IsSelected") == true)
                {
                    AppDataCenter.Singleton.FeaturesSelected.First(x => x.Name == Name).IsSelected = value;
                }
            }
        }

        public bool IsCalculate
        {
            get
            {
                return AppDataCenter.Singleton.IsFeatureCalculate(Name);
            }
            set
            {
                if (value == true && AppDataCenter.Singleton.IsFeatureCalculate(Name) == false)
                {
                    AppDataCenter.Singleton.AddFeatureCalculate(Name);
                    OnPropertyChanged("IsCalculate");
                }
                else if (value == false && AppDataCenter.Singleton.IsFeatureCalculate(Name) == true)
                {
                    AppDataCenter.Singleton.RemoveFeatureCalculate(Name);
                    OnPropertyChanged("IsCalculate");
                }
            }
        }
    }

    public class BaseConfigurationViewModel : SectionViewModel
    {
        public BaseConfigurationViewModel()
        {
            RegisterProperty("InputFolder", NotifyGroup.Configuration);
            RegisterProperty("NoOfWords", NotifyGroup.Configuration);
            RegisterProperty("Extended", NotifyGroup.Configuration);
            RegisterProperty("UseNonGoldenClass", NotifyGroup.Configuration);
            RegisterProperty("PagesMinMatch", NotifyGroup.Configuration);
            RegisterProperty("PagesMaxFp", NotifyGroup.Configuration);
            RegisterProperty(new Action( () => UpdateFeatures() ), NotifyGroup.Configuration);
            RegisterProperty(new Action(() => Update()), NotifyGroup.StatisticData);
            RegisterProperty(new Action(() => Update()), NotifyGroup.Configuration);

            UpdateFeatures();
            Update();
           
            
            ProbablityCreteria = 0;
            ProbablityCreteria2 = 0;
            MinimumSeperation = 0;
        }

        public override bool Enable
        {
            get
            {
                return AppDataCenter.Singleton.IsRunning == false;
            }
        }
        public override bool IsValid
        {
            get
            {
                return AppDataCenter.Singleton.IsCongigurationValid;
            }
        }

        public string InputFolder
        {
            get
            {
                return AppDataCenter.Singleton.InputFolder;
            }
            set
            {
                if (AppDataCenter.Singleton.InputFolder != value)
                {
                    AppDataCenter.Singleton.BuildSetupData(value, AppDataCenter.Singleton.GetSetupDataFileName(value));
                }
            }
        }


        void Update()
        {
            if ( m_isFeaturesResultExist != AppDataCenter.Singleton.SetupData.Pages.Any(x => x.DocData.Candidates != null) )
            {
                m_isFeaturesResultExist = !m_isFeaturesResultExist;
                OnPropertyChanged("IsFeaturesResultExist");
            }
        }
        bool m_isFeaturesResultExist;
        public bool IsFeaturesResultExist
        {
            get
            {
                return m_isFeaturesResultExist;
            }
        }

        protected void OnChange<T>(T member, T newValue, Action<T> setMember)
        {
            if (member.Equals(newValue) == false)
            {
                setMember(newValue);
                AppDataCenter.Singleton.NotifyChange(NotifyGroup.Configuration);
            }
        }


        public int NoOfWords
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.FeatureExtraction.NoOfWords;
            }
            set
            {
                OnChange(NoOfWords, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.FeatureExtraction.NoOfWords = x);
            }
        }
        public bool Extended
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.FeatureExtraction.Extended;
            }
            set
            {
                OnChange(Extended, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.FeatureExtraction.Extended = x);
            }
        }

        public bool UseNonGoldenClass
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.FeatureExtraction.UseNonGoldenClass;
            }
            set
            {
                OnChange(UseNonGoldenClass, value, x => AppDataCenter.Singleton.SetupData.BaseClassifierConfiguration.FeatureExtraction.UseNonGoldenClass = x);
            }
        }

        public int PagesMinMatch
        {
            get
            {
                return (int)(AppDataCenter.Singleton.AcceptanceCriteria.ModelMinMatchPages * 100d);
            }
            set
            {
                OnChange(PagesMinMatch, value, x => AppDataCenter.Singleton.AcceptanceCriteria.ModelMinMatchPages = (double)x / 100d);
            }
        }


        public int ProbablityCreteria
        {
            get
            {
                return (int)(AppDataCenter.Singleton.AcceptanceCriteria.ProbablityCreteria );
            }
            set
            {
                OnChange(ProbablityCreteria, value, x => AppDataCenter.Singleton.AcceptanceCriteria.ProbablityCreteria = (double)x);
            }
        }

        public int ProbablityCreteria2
        {
            get
            {
                return (int)(AppDataCenter.Singleton.AcceptanceCriteria.ProbablityCreteria2 );
            }
            set
            {
                OnChange(ProbablityCreteria2, value, x => AppDataCenter.Singleton.AcceptanceCriteria.ProbablityCreteria2 = (double)x);
            }
        }


        public int MinimumSeperation
        {
            get
            {
                return (int)(AppDataCenter.Singleton.AcceptanceCriteria.MinimumSeperation);
            }
            set
            {
                OnChange(MinimumSeperation, value, x => AppDataCenter.Singleton.AcceptanceCriteria.MinimumSeperation = (double)x);
            }
        }


        


        public int PagesMaxFp
        {
            get
            {
                return (int)(AppDataCenter.Singleton.AcceptanceCriteria.ModelMaxFpPages * 100d);
            }
            set
            {
                OnChange(PagesMaxFp, value, x => AppDataCenter.Singleton.AcceptanceCriteria.ModelMaxFpPages = (double)x / 100d);
            }
        }

        void UpdateFeatures()
        {
            var remove = m_featuresSelected
                            .Where(x => AppDataCenter.Singleton.FeaturesSelected.Any(y => y.Name == x.Name) == false)
                            .ToList();

            // Remove old features
            foreach (var item in remove)
            {
                m_featuresSelected.Remove(item);
            }

            // Add or Update features list
            foreach (var item in AppDataCenter.Singleton.FeaturesSelected)
            {
                FeatureViewModel feature = m_featuresSelected.FirstOrDefault(x => x.Name == item.Name);
                if ( feature == null )
                {
                    feature = new FeatureViewModel(item.Name);
                    m_featuresSelected.Add(feature);
                }
                feature.IsSelected = item.IsSelected;
            }

            OnPropertyChanged("FeaturesSelected");
        }

        ObservableCollection<FeatureViewModel> m_featuresSelected = new ObservableCollection<FeatureViewModel>();
        public IEnumerable<FeatureViewModel> FeaturesSelected
        {
            get
            {
                return m_featuresSelected;
            }
        }

      

     


    }
}

