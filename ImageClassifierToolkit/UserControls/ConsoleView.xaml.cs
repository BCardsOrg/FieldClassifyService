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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls
{
    /// <summary>
    /// Interaction logic for PagesView.xaml
    /// </summary>
    public partial class ConsoleView : UserControl
    {
        public ConsoleView()
        {
            InitializeComponent();

            DataContext = new ConsoleViewModel();
            ((INotifyCollectionChanged)FeatureHolder.Items).CollectionChanged += ListView_CollectionChanged;
        }
        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // scroll the new item into view   
                Scroller1.ScrollToEnd();
                //listView.ScrollIntoView(e.NewItems[0]);
            }
        }

    }
}
