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

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls
{
    /// <summary>
    /// Interaction logic for PagesView.xaml
    /// </summary>
    public partial class PagesView : UserControl
    {
        public PagesView()
        {
            InitializeComponent();

            DataContext = new PagesViewModel();
        }
        private void pagesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as PagesViewModel;

            // Because we have listView per gold class, then simulate the selected behavior on on ListView
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                // No ModifierKeys, so all other selected pages should be unselected
                if (e.AddedItems.Count > 0)
                {
                    var othreSelectedPages = PagesViewModel.Pages
                                                    .SelectMany(x => x.Pages)
                                                    .Where(x => x.IsSelected == true)
                                                    .Except(e.AddedItems.Cast<PageViewModel>())
                                                    .ToList();

                    foreach (var page in othreSelectedPages)
                    {
                        page.IsSelected = false;
                    }
                }
            }

            vm.UpdateSelectedPages();
        }

    }
}
