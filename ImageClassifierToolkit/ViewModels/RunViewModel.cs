using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{

    public class RunViewModel : SectionViewModel
    {
        public RunViewModel()
        {
            RegisterProperty("FieldsSelected", NotifyGroup.Configuration);
        }
        public override bool Enable
        {
            get
            {
                return AppDataCenter.Singleton.IsRunning == false &&
                    AppDataCenter.Singleton.IsTrainTestValid;
            }
        }
        public override bool IsValid
        {
            get
            {
                return AppDataCenter.Singleton.IsRunValid;
            }
        }


        public ObservableCollection <SetupFieldViewModel> FieldsSelected
        {
            get
            {
                return new ObservableCollection<SetupFieldViewModel>(
                    AppDataCenter.Singleton.SetupData.FilteredFields.Where(x => x.NumApear > 0).OrderByDescending(a => a.NumApear).Select(x => new SetupFieldViewModel(x)));
            }
            set
            {
                AppDataCenter.Singleton.SetupData.FilteredFields = value.Select(a => a.SetupFieldDataProp);
            }
        }

        protected void OnChange<T>(T member, T newValue, Action<T> setMember)
        {
            if (member.Equals(newValue) == false)
            {
                setMember(newValue);
                AppDataCenter.Singleton.NotifyChange(NotifyGroup.Configuration);
            }
        }


      
    }
}
