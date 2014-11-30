using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    public class SelectTrainDataCommand : CommandBaseAsyncUI
    {
        public bool MixClasses { get; set; }
        public SelectTrainDataCommand() 
			: base(Shell.ShellDispatcher)
		{
            MixClasses = false;
        }

        protected override void LoadData(object state)
        {
        }

        protected override void UpdateView(object state)
        {
        }

        protected override void UpdateData(object state)
        {
            if (state != null && state is double)
            {
                AppDataCenter.Singleton.SelectTrainPages((double)state, MixClasses);
            }
           
        }
    }
}
