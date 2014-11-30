
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using TiS.Recognition.FieldClassifyService.Service;
using TiS.Recognition.FieldClassifyService.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class ModelsListViewModel : BaseModelView
    {
        public ModelsListViewModel()
        {
            RegisterProperty(new Action(() => UpdateModelsList() ), NotifyGroup.Configuration);

            UpdateModelsList();
        }

        ModelDataInfo m_selectedModel;
        public ModelDataInfo SelectedModel
        {
            get
            {
                return m_selectedModel;
            }
            set
            {
                if ( OnChange(ref m_selectedModel, value, "SelectedModel") == true )
                {
                    OnPropertyChanged("IsModelSelected");
                }
            }
        }

        public bool IsModelSelected
        {
            get
            {
                return m_selectedModel != null;
            }
        }

        public string ModelFolder
        {
            get
            {
                return ModelsService.Service.ModelsFolder;
            }
            set
            {
                if (ModelsService.Service.ModelsFolder != value)
                {
                    AppDataCenter.Singleton.SetModelFolder(value);
                    OnPropertyChanged("ModelFolder");
                }
            }
        }

        ObservableCollection<ModelDataInfo> m_models = new ObservableCollection<ModelDataInfo>();
        public IEnumerable<ModelDataInfo> Models
        {
            get
            {
                return m_models;
            }
        }

        private void UpdateModelsList()
        {
            m_models.Clear();
            foreach (var modelInfo in ModelsService.Service.ModelsInfo)
            {
                m_models.Add(modelInfo);
            }
        }
    }
}
