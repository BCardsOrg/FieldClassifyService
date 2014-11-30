using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class StatusBarViewModel : BaseModelView
    {
        public StatusBarViewModel()
        {
            RegisterProperty("IsRunning", NotifyGroup.Progress);
            RegisterProperty("NoOfPages", NotifyGroup.Configuration);
            RegisterProperty("NoOfPages", NotifyGroup.Progress);

        }
        public bool IsRunning
        {
            get
            {
                return AppDataCenter.Singleton.IsRunning;
            }
        }

        public int NoOfPages
        {
            get
            {
                return AppDataCenter.Singleton.SetupData.Pages.Count();
            }
        }
    }
}
