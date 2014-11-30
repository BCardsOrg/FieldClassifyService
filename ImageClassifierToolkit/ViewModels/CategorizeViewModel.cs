using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class CategorizeViewModel : SectionViewModel
    {
        public override bool Enable
        {
            get
            {
                return AppDataCenter.Singleton.IsRunning == false &&
                    AppDataCenter.Singleton.IsCongigurationValid;
            }
        }
        public override bool IsValid
        {
            get
            {
                return AppDataCenter.Singleton.IsCategorizeValid;
            }
        }
        
        public CategorizeViewModel()
        {
            RegisterProperty(new Action(() => UpdateGoldClassNames()), NotifyGroup.Configuration);
            RegisterProperty("Fields", NotifyGroup.Configuration);
        }

        public void SetGoldClass(string goldClassName)
        {
            PagesViewModel.SetGoldClass(goldClassName);
        }

        public string SelectedGoldClassName { get; set; }

        ObservableCollection<string> m_goldClassNames = new ObservableCollection<string>();

        public IEnumerable<string> GoldClassNames
        {
            get
            {
                return m_goldClassNames;
            }
        }

        public IEnumerable<string> Fields
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.Fields.Select(x => x.Name);
            }
        }

        public string SelectedClassifierField { get; set; }

        void UpdateGoldClassNames()
        {
            m_goldClassNames.Clear();

            // Take all gold classes
            foreach (var item in AppDataCenter.Singleton.SetupData.Pages
                                                 .Where(x => string.IsNullOrEmpty(x.Setup.ClassName) == false)
                                                 .Select(x => x.Setup.ClassName)
                                                 .Distinct())
            {
                m_goldClassNames.Add(item);
            }

            // Add recognition class
            if (AppDataCenter.Singleton.BaseReportData != null)
            {
                foreach (var item in AppDataCenter.Singleton.BaseReportData.PagesReportData
                                        .Where(x => x.ClassMatch.Count > 0 &&
                                            string.IsNullOrEmpty(x.ClassMatch.First().Key) == false)
                                        .Select(x => x.ClassMatch.First().Key)
                                        .Distinct())
                {
                    if (m_goldClassNames.Contains(item) == false)
                    {
                        m_goldClassNames.Add(item);
                    }
                }
            }
        }
    }
}
