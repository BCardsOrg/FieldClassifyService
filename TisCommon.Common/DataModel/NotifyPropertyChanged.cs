using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;

namespace TiS.Core.TisCommon.DataModel
{
	[Serializable]
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public System.Windows.Threading.Dispatcher Dispatcher { get; set; }

		protected bool OnChange<T>(ref T member, T value, string name)
		{
			if ((member != null) && (member.Equals(value) == true))
				return false;
			if ((member == null) && (value == null))
				return false;
			member = value;
			OnPropertyChanged(name);
			return true;
		}


		[field: IgnoreDataMember]
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				if (Dispatcher == default(System.Windows.Threading.Dispatcher) )
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
				else
				{
					Dispatcher.BeginInvoke(new WaitCallback(delegate
					{
						if (PropertyChanged != null)
						{
							try
							{
								PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
							}
							catch {}
						}
					}), this);
				}
			}
		}
	}
}
