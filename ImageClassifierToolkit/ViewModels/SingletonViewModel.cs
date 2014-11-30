using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels
{
	public class SingletonViewModel<T> : NotifyPropertyChanged where T : new()
	{
		public SingletonViewModel()
		{
			base.Dispatcher = Shell.ShellDispatcher;
		}

		static T m_singleton = new T();

		static public T Singleton
		{
			get
			{
				return m_singleton;
			}
		}
	}
}
