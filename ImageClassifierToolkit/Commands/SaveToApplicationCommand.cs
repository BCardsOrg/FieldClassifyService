using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
/*using TiS.Core.Application;
using TiS.Core.Application.Interfaces;
using TiS.Core.TisCommon.Attachment.File;*/

using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using TiS.Recognition.FieldClassifyService.Common.Dialogs;
using TiS.Recognition.FieldClassifyService.Common.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{

  /*  class SaveToApplicationCommand : BaseAppModelCommand
    {
        protected override BaseModelsListDlg GetFialog()
        {
            return new SaveModelDlg(ModelView);
        }

        protected override void OnOk()
        {
            if (string.IsNullOrEmpty(ModelView.ModelName) == false)
            {
                string modelName = (string)GetParameter();

                var modelData = ModelsService.Service.Load(modelName);

                modelData.Name = ModelView.ModelName;

                using  ( MemoryStream stream = new MemoryStream() )
                {
                    ModelsService.Service.Save(modelData, stream);

                    stream.Position = 0;

                    Csm.ApplicationServices[ModelView.CurrentApplication].
                        SetupAttachment.SaveBLOBAsAttachment(
                            stream,
                            ModelView.ModelName + "." + Const.ModelFileExt, 
                            TIS_ATTACHMENT_EXISTS_ACTION.TIS_EXISTING_OVERRIDE);
                }

            }
        }
    }*/
}
