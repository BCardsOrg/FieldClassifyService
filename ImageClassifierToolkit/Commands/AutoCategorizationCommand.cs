using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    class AutoCategorizationCommand : CommandBaseAsyncUI
    {
        public string ClassifierField { get; set; }
        public AutoCategorizationCommand() :
            base(Shell.ShellDispatcher)
        { }
        protected override void LoadData(object state)
        {
            AppDataCenter.Singleton.SetPagesClass(ClassifierField);
        }

        protected override void UpdateView(object state)
        {
        }

        protected override void UpdateData(object state)
        {
        }
    }
}
