using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
    public abstract class BaseModelView : NotifyPropertyChanged
    {
		/// <summary>
		/// Gets the property changed map.
		/// This map bind between ViewModel property to data center property name (or property group name)
		/// </summary>
        private Dictionary<string, List<object>> m_propertyChangedMap;

		public BaseModelView()
		{
            m_propertyChangedMap = new Dictionary<string, List<object>>();


			Dispatcher = Shell.ShellDispatcher;
			AppDataCenter.Singleton.PropertyChanged += new PropertyChangedEventHandler(AppDataCenter_PropertyChanged);
		}

		#region Protected methods
		/// <summary>
		/// Raises data changed notification
		/// </summary>
		protected void AppDataCenter_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Shell.ShellDispatcher == null) return;

			// Find the property name we need to notify
            List<object> properties;

            if (m_propertyChangedMap.TryGetValue(e.PropertyName, out properties) == true)
            {
                foreach (var property in properties)
                {
                    if (property is string)
                    {
                        OnPropertyChanged(property.ToString());
                    }
                    else if ( property is Action )
                    {
                        (property as Action)();
                    }
                };
            }
		}

        public void RegisterProperty(object propertyName, string groupName)
        {
            List<object> properties;

            if (m_propertyChangedMap.TryGetValue(groupName, out properties) == false)
            {
                properties = new List<object>();
                m_propertyChangedMap.Add(groupName, properties);
            }

            properties.Add(propertyName);
        }

        #endregion
    }
}

