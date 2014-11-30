using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;


namespace TiS.Recognition.FieldClassifyService.Common.ViewModels
{
    public abstract class ClassifierModelsViewModel : NotifyPropertyChanged
    {
        public IEnumerable<string> Applications { get; set; }

        string m_currentApplication = null ;
        public string CurrentApplication
        {
            get
            {
                return m_currentApplication;
            }
            set
            {
                if (OnChange(ref m_currentApplication, value, "CurrentApplication") == true)
                {
                    UpdateModels(m_currentApplication);
                    OnPropertyChanged("Models");
                }
            }
        }

        public IEnumerable<string> Models { get; set; }

        public string ModelName { get; set; }

        abstract public void UpdateModels(string appName);
        
    }
}
