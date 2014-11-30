using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
/*using TiS.Core.Application;
using TiS.Core.Application.Interfaces;*/

using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using TiS.Recognition.FieldClassifyService.Common.Dialogs;
using TiS.Recognition.FieldClassifyService.Common.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
    /*class LoadFromApplicationCommand : BaseAppModelCommand
    {
      (  protected override BaseModelsListDlg GetFialog()
        {
          return new SelectModelDlg(ModelView);
        }

        protected override void OnOk()
        {
           // if (string.IsNullOrEmpty(ModelView.ModelName) == false && string.IsNullOrEmpty(ModelsService.Service.ModelsFolder) == false)
           // {
               var modelFileName = Path.Combine( ModelsService.Service.ModelsFolder, ModelView.ModelName + "." + Const.ModelFileExt);

                Csm.ApplicationServices[ModelView.CurrentApplication]
                    .SetupAttachment.GetAttachment(modelFileName);

                ModelsService.Service.UpdateModelsInfo();

                AppDataCenter.Singleton.LoadModel(ModelView.ModelName);
          //  }
        }
    }*/
}
