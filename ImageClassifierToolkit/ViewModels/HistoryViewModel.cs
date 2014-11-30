using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class HistoryViewModel : SectionViewModel
    {
        public HistoryViewModel()
        {
            ModelsViewModel = new ModelsListViewModel();
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
                return string.IsNullOrEmpty(ModelsService.Service.ModelsFolder) == false;
            }
        }

        public ModelsListViewModel ModelsViewModel { get; private set; }
    }
}
