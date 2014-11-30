using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public abstract class SectionViewModel : BaseModelView
    {
        public SectionViewModel()
        {
            RegisterProperty("IsNotRunnig", NotifyGroup.StatisticData);
            RegisterProperty("IsValid", NotifyGroup.Configuration);
            RegisterProperty("Enable", NotifyGroup.Configuration);
            RegisterProperty("Enable", NotifyGroup.StatisticData);
        }

        public abstract bool Enable { get; }
        public abstract bool IsValid { get; }

        public virtual bool IsShowPagesView
        {
            get
            {
                return true;
            }
        }

        public bool IsNotRunnig
        {
            get
            {
                return AppDataCenter.Singleton.IsRunning == false;
            }
        }
    }
}
