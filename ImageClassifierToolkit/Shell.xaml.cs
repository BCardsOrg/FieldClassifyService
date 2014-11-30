using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using ImageClassifierToolkit;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit
{
	/// <summary>
	/// Interaction logic for Shell.xaml
	/// </summary>
	public partial class Shell : Window
	{
		private static Shell m_Instance = null;

		public static Dispatcher ShellDispatcher
		{
			get
			{
				if (m_Instance != null)
				{
					return m_Instance.Dispatcher;
				}
				else
				{
					return Dispatcher.FromThread(Thread.CurrentThread);
				}
			}
		}

		public Shell()
		{
			InitializeComponent();
		}

        public static void JumpToReportView()
        {
            if (m_Instance != null)
            {
                m_Instance.tabCntr.SelectedItem = m_Instance.reportHolder;
            }
            else
            {
                var shell = App.Current.MainWindow as Shell;
                if (shell != null)
                {
                    shell.tabCntr.SelectedItem = shell.reportHolder;
                }
            }
        }

        private void tabCntr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( e.AddedItems.Count >= 1 && e.AddedItems[0] is TabItem )
            {
                var tab = e.AddedItems[0] as TabItem;
                var tabC = sender as TabControl;
                (tabC.DataContext as SectionsViewModel).SelectedSection = tab.DataContext as SectionViewModel;
            }
        }

	}
}
