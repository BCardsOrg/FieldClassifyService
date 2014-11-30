using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Statistic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using OxyPlot;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls.OxyPlot;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{

    public class ConsoleViewModel : SectionViewModel
	{
        public ConsoleViewModel()
		{
          
            RegisterProperty("ConsoleMessage", NotifyGroup.Console);
            
            
		}

        public override bool Enable
        {
            get
            {
                return AppDataCenter.Singleton.IsRunValid;
            }
        }
        public override bool IsValid
        {
            get
            {
                return true;
            }
        }
        public ObservableCollection<string> ConsoleMessage
        {
            get
            {
                return (AppDataCenter.Singleton.ConsoleMessage);
            }
            set
            {
                OnChange(ConsoleMessage, value, x => AppDataCenter.Singleton.ConsoleMessage = x);
            }
        }

        

        

        protected void OnChange<T>(T member, T newValue, Action<T> setMember)
        {
            if (member.Equals(newValue) == false)
            {
                setMember(newValue);
                AppDataCenter.Singleton.NotifyChange(NotifyGroup.StatisticData);
            }
        }
	}
}
