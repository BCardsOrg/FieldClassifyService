
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class SectionsViewModel : BaseModelView
    {
        public SectionsViewModel()
        {
           // switch (FactoryServices.SvmEngineType)
          //  {
               // case SvmEngine.Svm:
                    Configuration = new AccordConfigurationViewModel();
                   // break;
               // case SvmEngine.SvmNet:
                 //   Configuration = new SvmNetConfigurationViewModel();
                  //  break;
          //  }
            
         
            TrainTest = new TrainTestViewModel();
            Categorize = new CategorizeViewModel();
            Run = new RunViewModel();
            Report = new ReportViewModel();
            Analyse = new AnalyseViewModel();
            AnalyseNum = new AnalyseNumViewModel();
            History = new HistoryViewModel();
        }
        public BaseConfigurationViewModel Configuration { get; private set; }

        public CategorizeViewModel Categorize { get; private set; }

        public TrainTestViewModel TrainTest { get; private set; }

        public RunViewModel Run { get; private set; }

        public ReportViewModel Report { get; private set; }

        public AnalyseViewModel Analyse { get; private set; }
        public AnalyseNumViewModel AnalyseNum { get; private set; }

        public HistoryViewModel History { get; private set; }

        SectionViewModel m_selectedSection;
        public SectionViewModel SelectedSection
        {
            get
            {
                return m_selectedSection;
            }
            set
            {
                if (OnChange(ref m_selectedSection, value, "SelectedSection") == true)
                {
                    OnPropertyChanged("IsShowPagesView");
                }
            }
        }

        public virtual bool IsShowPagesView
        {
            get
            {
                return m_selectedSection == null ? true : m_selectedSection.IsShowPagesView;
            }
        }

    }
}
