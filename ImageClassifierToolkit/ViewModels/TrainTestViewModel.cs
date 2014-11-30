using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public class TrainTestViewModel : SectionViewModel
    {
        public TrainTestViewModel()
        {
            TrainSize = 90;
        }

        public override bool Enable
        {
            get
            {
                return AppDataCenter.Singleton.IsRunning == false && AppDataCenter.Singleton.IsCongigurationValid;
            }
        }
        public override bool IsValid
        {
            get
            {
                // Check we have train pages at least 2
                return AppDataCenter.Singleton.IsTrainTestValid;
            }
        }

        public bool IsMixTrain { get; set; }

        public double TrainSize { get; set; }
    }
}
