using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    class SetClassPerFolderCommand : CommandBaseAsync
    {
        protected override void DoWork(object parameter)
        {
            AppDataCenter.Singleton.SetPageCalssPerFolder();
        }
    }
}
