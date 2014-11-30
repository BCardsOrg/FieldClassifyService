using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*using TiS.Core.Application;
using TiS.Core.Application.Interfaces;
using TiS.Core.TisCommon.Attachment.File;*/
using TiS.Recognition.FieldClassifyService.Common.Data;
using TiS.Recognition.FieldClassifyService.Common.Dialogs;
using TiS.Recognition.FieldClassifyService.Common.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Commands
{
  /*  class BaseModelDlgVM : ClassifierModelsViewModel
    {
        ITisClientServicesModule m_csm;

        public BaseModelDlgVM(ITisClientServicesModule csm)
        {
            m_csm = csm;
            Init();
        }

        void Init()
        {
            Applications = m_csm.ActiveApplications.ToList();
        }

        public override void UpdateModels(string appName)
        {
            Models = m_csm.ApplicationServices[appName]
                                .SetupAttachment
                                .QueryAttachments(Const.ModelFileExt)
                                .Select(x => System.IO.Path.GetFileNameWithoutExtension(x))
                                .ToList();
        }
    }
    */
    abstract class BaseAppModelCommand : CommandBaseDialog
    {
       // protected ITisClientServicesModule Csm { get; private set; }

       // protected BaseModelDlgVM ModelView { get; set; }

        static string m_lastAppName = null;

        bool m_result = false;

        public BaseAppModelCommand()
            : base(Shell.ShellDispatcher)
        {
           /* Csm = TisClientServicesModule.GetSingletoneInstance();
            if (Csm.Initialized == false)
            {
                Csm.Initialize("", "", "", 0, "", null, null, "", "", "", "");
            }

            if (Csm.Initialized == false)
                throw new Exception("Csm.Initialize fail");*/
        }

        protected abstract BaseModelsListDlg GetFialog();
        protected abstract void OnOk();

       /* protected override void ShowDialog()
        {
            ModelView = new BaseModelDlgVM(Csm) { CurrentApplication = m_lastAppName };

            var dlg = GetFialog();

            if ( dlg.ShowDialog() == true )
            {
                m_result = true;
            }

            m_lastAppName = ModelView.CurrentApplication;
        }*/

        protected override void UpdateData()
        {
            if (m_result == true)
            {
                OnOk();
            }
          //  Csm.Dispose();
        }
    }
}
